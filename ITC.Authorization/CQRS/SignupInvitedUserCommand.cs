using System.ComponentModel.DataAnnotations;
using ITC.Authorization.CQRS.InviteUsers;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.Authorization.ServiceBus.Organization;
using ITC.CQRS.Base.Get;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class SignupInvitedUserCommand : IRequest
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public string Password { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string InviteToken { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }

    public class Handler : IRequestHandler<SignupInvitedUserCommand>
    {
        private readonly IServiceBusMessagePublisher<EntityUpdateEvent> _entityUpdateEventMqPublisher;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceBusMessagePublisher<SendEmailWithVerificationCodeMq> _sendEmailWithVerificationCodePublisher;
        private readonly KeycloakClient _keycloakClient;
        private readonly KeycloakClientOptions _keycloakOptions;

        public Handler(
            IServiceBusMessagePublisher<EntityUpdateEvent> entityUpdateEventMqPublisher,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            IServiceBusMessagePublisher<SendEmailWithVerificationCodeMq> sendEmailWithVerificationCodePublisher,
            KeycloakClient keycloakClient,
            IOptions<KeycloakClientOptions> keycloakOptions)
        {
            _entityUpdateEventMqPublisher = entityUpdateEventMqPublisher;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _sendEmailWithVerificationCodePublisher = sendEmailWithVerificationCodePublisher;
            _keycloakClient = keycloakClient;
            _keycloakOptions = keycloakOptions.Value;
        }

        public async Task Handle(SignupInvitedUserCommand request, CancellationToken cancellationToken)
        {
            var invitedUserQuery = new Get
            {
                InviteToken = request.InviteToken
            };

            UserApiGetCollectionRequestWrapper<Get, InvitedUserDto> request2 = new UserApiGetCollectionRequestWrapper<Get, InvitedUserDto>(invitedUserQuery, _httpContextAccessor.HttpContext);
            var invitedUsers = await _mediator.Send(request2, cancellationToken);
            if (invitedUsers == null)
                throw new ValidationException($"Не найден приглашенный пользователь с токен: {request.InviteToken}");

            var invitedUser = invitedUsers.Items.FirstOrDefault();
            if (invitedUser == null)
                throw new ValidationException($"Не найден приглашенный пользователь с токен: {request.InviteToken}");

            var createdUser = await _mediator.Send(new GetUserQuery(invitedUser.UserId ?? Guid.Empty), cancellationToken);
            if (createdUser == null)
                throw new ValidationException($"Не найден приглашенный пользователь с токен: {request.InviteToken}");

            await ConfirmEmail(createdUser, cancellationToken);
            await SetCredentials(createdUser, request, cancellationToken);
            await AddEmployee(invitedUser, request, createdUser, cancellationToken);
        }

        private async Task SetCredentials(User createdUser, SignupInvitedUserCommand signupInvitedUserCommand, CancellationToken cancellationToken)
        {
            var result = await _keycloakClient.SetUserPasswordAsync(_keycloakOptions.Realm, createdUser.Id.ToString(), signupInvitedUserCommand.Password, cancellationToken);
            
            if (!result.Success)
            {
                Log.Warn("Error when changing the password {0}", result.Error);
                throw new ValidationException("Ошибка при смене пароля");
            }
        }
        
        private async Task ConfirmEmail(User createdUser, CancellationToken cancellationToken)
        {
            createdUser.EmailVerified = true;
            var command = new KeyCloakUpdateUserCommand
            {
                User = createdUser
            };
            await _mediator.Send(command, cancellationToken);
        }
        
        private async Task AddEmployee(InvitedUserDto invitedUserDto, SignupInvitedUserCommand signupInvitedUserCommand, User createdUser, CancellationToken cancellationToken)
        {
            var command = new AddEmployeeFromInvitedUserMq
            {
                InviteToken = signupInvitedUserCommand.InviteToken,
                UserId = createdUser.Id,
                Phone = signupInvitedUserCommand.Phone
            };

            await _entityUpdateEventMqPublisher.Produce(new EntityUpdateEvent
            {
                ActionType = "AddEmployeeFromInvitedUserMq",
                EntityType = "Employee",
                Payload = JsonConvert.SerializeObject(command)
            }, cancellationToken: cancellationToken);
        }

        private int GenerateVerificationCode()
        {
            return Random.Shared.Next(100000, 999999);
        }

        private async Task SendEmailWithVerificationCodeMq(string email, int verificationCode, CancellationToken cancellationToken)
        {
            var sendEmailWithVerificationCodeMqCommand = new SendEmailWithVerificationCodeMq
            {
                VerificationCode = verificationCode.ToString(),
                Email = email
            };

            await _sendEmailWithVerificationCodePublisher.Produce(sendEmailWithVerificationCodeMqCommand, cancellationToken: cancellationToken);
        }
    }
}
