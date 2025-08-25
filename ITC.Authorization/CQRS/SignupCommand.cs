using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Flurl.Http;
using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.CQRS.Base;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;
using ValidationException = FluentValidation.ValidationException;

namespace ITC.Authorization.CQRS;

public class SignupCommand : IRequest
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    [Required] public string UserId { get; set; } = "";
    public string? Password { get; set; }
    [Required] public Guid? EmployeeId { get; set; }
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string FirstName { get; set; } = "";
    [Required] public string LastName { get; set; } = "";
    public string? Role { get; set; }
    public bool NeedEmailVerification { get; set; } = true;

    public class Handler : IRequestHandler<SignupCommand>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;

        private readonly IServiceBusMessagePublisher<SendEmailWithVerificationCodeMq>
            _sendEmailWithVerificationCodePublisher;

        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(KeycloakClient keycloak,
            IOptions<KeycloakClientOptions> options,
            IServiceBusMessagePublisher<SendEmailWithVerificationCodeMq> sendEmailWithVerificationCodePublisher,
            IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _keycloak = keycloak;
            _clientOptions = options.Value;
            _sendEmailWithVerificationCodePublisher = sendEmailWithVerificationCodePublisher;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var createdUser = await CreateUser(request, cancellationToken);
        }

        private async Task<User> CreateUser(SignupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var verificationCode = GenerateVerificationCode();

                Log.Warn($"{request.Email} verification code {verificationCode}"); //TODO

                var user = new User
                {
                    UserName = request.UserId,
                    Enabled = true,
                    EmailVerified = false,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Attributes = new Dictionary<string, IEnumerable<string>>(),

                    //TODO Не работает. Нужно сделать через шину данных
                    //https://stackoverflow.com/questions/71317507/how-can-i-create-user-with-multiple-client-roles-in-a-single-api
                    //https://stackoverflow.com/questions/56254627/using-admin-api-to-add-client-role-to-user
                    ClientRoles = new Dictionary<string, object> { { _clientOptions.ClientName, new[] { "user" } } },
                };

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.Credentials =
                        [new Credentials { Type = "Password", Value = request.Password, Temporary = false }];
                }
                else
                {
                    user.RequiredActions = new ReadOnlyCollection<string>(["UPDATE_PASSWORD"]);
                }

                if (request.NeedEmailVerification)
                    user.Attributes.Add("emailverificationcode", new List<string> { verificationCode.ToString() });

                if (request.EmployeeId.HasValue)
                    user.Attributes["employeeid"] = [request.EmployeeId.ToString()];

                var isSuccess = await _keycloak.CreateUserAsync(_clientOptions.Realm, user, cancellationToken);
                if (isSuccess)
                {
                    var roles = request.Role?.Split(';');
                    var createdUser = await GetUserByEmail(request.Email, cancellationToken);
                    await SetUserRoles(createdUser, roles, cancellationToken);
                    if (request.NeedEmailVerification)
                        await SendEmailWithVerificationCodeMq(request.Email, verificationCode, cancellationToken);
                }

                return user;
            }
            catch (FlurlHttpException e)
            {
                if (e.StatusCode == 409)
                    throw new ValidationException("Пользователь с таким именем/email уже существует");

                throw;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        private int GenerateVerificationCode()
        {
            return Random.Shared.Next(100000, 999999);
        }

        private async Task SetUserRoles(User user, string[]? roles, CancellationToken cancellationToken)
        {
            var command = new UserSetRolesCommand
            {
                Roles = roles ??
                [
                    "user"
                ],
                UserId = user.Id
            };
            await _mediator.Send(
                new UserApiRequestWrapper<UserSetRolesCommand>(command,
                    _httpContextAccessor.HttpContext ?? new DefaultHttpContext()), cancellationToken);
        }

        private async Task<User> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            var query = new GetUsersQuery
            {
                Search = email
            };

            var users = await _mediator.Send(query, cancellationToken);
            var user = users.Items.FirstOrDefault();
            if (user == null)
                throw new ValidationException($"Не найден пользователь с email: {email}");
            return user;
        }

        private async Task SendEmailWithVerificationCodeMq(string email, int verificationCode,
            CancellationToken cancellationToken)
        {
            var sendEmailWithVerificationCodeMqCommand = new SendEmailWithVerificationCodeMq
            {
                VerificationCode = verificationCode.ToString(),
                Email = email
            };

            await _sendEmailWithVerificationCodePublisher.Produce(sendEmailWithVerificationCodeMqCommand,
                cancellationToken: cancellationToken);
        }
    }
}