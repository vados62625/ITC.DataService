using System.ComponentModel.DataAnnotations;
using ITC.Authorization.CQRS.InviteUsers;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.Authorization.Enums;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.CQRS.Base;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class AddInvitedUserCommand : IRequest<CreatedInvitedUser>
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid? EmployeeId { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// Пол
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Должность
    /// </summary>
    public string? Position { get; set; } = string.Empty;
    
    public string Role { get; set; } = string.Empty;
    
    public class Handler : IRequestHandler<AddInvitedUserCommand, CreatedInvitedUser>
    {
        private readonly IServiceBusMessagePublisher<SendEmailWithInviteTokenMq> _sendEmailWithInviteTokenMqPublisher;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(IMediator mediator, IServiceBusMessagePublisher<SendEmailWithInviteTokenMq> sendEmailWithInviteTokenMqPublisher, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _sendEmailWithInviteTokenMqPublisher = sendEmailWithInviteTokenMqPublisher;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreatedInvitedUser> Handle(AddInvitedUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var inviteToken = Guid.NewGuid().ToString();

                var createdUser = await SignUp(request, cancellationToken);
                var invitedUser = await AddInvitedUser(request, inviteToken, createdUser.Id, cancellationToken);
                
                await SendEmailWithInviteToken(request, inviteToken, cancellationToken);

                var result = new CreatedInvitedUser
                {
                    InviteToken = inviteToken,
                    UserId = createdUser.Id
                };

                return result;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        private async Task<InvitedUserDto> AddInvitedUser(AddInvitedUserCommand command, string inviteToken, Guid userId, CancellationToken cancellationToken)
        {
            var add = new Add
            {
                UserId = userId,
                EmployeeId = command.EmployeeId,
                InviteToken = inviteToken,
                Email = command.Email,
                Position = command.Position,
                FirstName = command.FirstName,
                LastName = command.LastName,
                MiddleName = command.MiddleName,
                Gender = command.Gender,
                Role = command.Role,
            };

            var request = new UserApiRequestWrapper<Add, InvitedUserDto>(add, _httpContextAccessor.HttpContext!);
            return await _mediator.Send(request, cancellationToken);
        }
        
        private async Task<User> SignUp(AddInvitedUserCommand addUserCommand, CancellationToken cancellationToken)
        {
            var command = new SignupCommand
            {
                EmployeeId = addUserCommand.EmployeeId,
                UserId = addUserCommand.Email,
                Email = addUserCommand.Email,
                FirstName = addUserCommand.FirstName,
                LastName = addUserCommand.LastName,
                Role = addUserCommand.Role,
                NeedEmailVerification = false
            };

            await _mediator.Send(command, cancellationToken);

            var query = new GetUsersQuery
            {
                Search = addUserCommand.Email
            };

            var users = await _mediator.Send(query, cancellationToken);
            var user = users.Items.FirstOrDefault();
            if (user == null)
                throw new ValidationException($"Не найден пользователь с email:{addUserCommand.Email}");

            return user;
        }

        private async Task SendEmailWithInviteToken(AddInvitedUserCommand request, string inviteToken, CancellationToken cancellationToken)
        {
            var message = new SendEmailWithInviteTokenMq()
            {
                Email = request.Email,
                InviteToken = inviteToken
            };

            await _sendEmailWithInviteTokenMqPublisher.Produce(message, cancellationToken: cancellationToken);
        }
    }
}