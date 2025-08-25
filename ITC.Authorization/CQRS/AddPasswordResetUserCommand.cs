using FluentValidation;
using ITC.Authorization.CQRS.PasswordResetUsers;
using ITC.Authorization.CQRS.PasswordResetUsers.Dto;
using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Get;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class AddPasswordResetUserCommand: IRequest
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public string Search { get; set; } = null!;
    public string PasswordResetTokenLink { get; set; } = null!;

    public class Handler(
        IMediator mediator,
        IServiceBusMessagePublisher<SendEmailWithPasswordResetLinkMq> sendEmailWithPasswordResetLinkPublisher,
        KeycloakClient keycloak,
        IOptions<KeycloakClientOptions> keycloakOptions,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<AddPasswordResetUserCommand>
    {
        private readonly KeycloakClientOptions _keycloakOptions = keycloakOptions.Value;

        public async Task Handle(AddPasswordResetUserCommand request, CancellationToken cancellationToken)
        {
            var user = await UserSearch(request.Search, cancellationToken);

            if (user != null)
            {
                await CheckTokenByUserId(user.Id, cancellationToken);
            
                var passwordResetToken = await AddPasswordResetUser(user.Id, cancellationToken);
                
                await SendEmail(user.Email, request.PasswordResetTokenLink + "/" + passwordResetToken, cancellationToken);
            } 
        }

        private async Task<User?> UserSearch(string search, CancellationToken cancellationToken)
        {
            var users = await keycloak.GetUsersAsync(
                _keycloakOptions.Realm,
                search: search,
                cancellationToken: cancellationToken);
            
            var searchToLower = search.ToLower();
            var user = users.FirstOrDefault(e =>
                e.Email.Equals(searchToLower, StringComparison.CurrentCultureIgnoreCase) || 
                e.UserName.Equals(searchToLower, StringComparison.CurrentCultureIgnoreCase));

            if (user != null) return user;
            
            Log.Warn("User {0} not found in realm {1}", search, _keycloakOptions.Realm);
            throw new ValidationException("Пользователь по логину или email не найден");
        }
        
        // проверяем, если уже есть токен для этого пользователя, то его удаляем
        private async Task CheckTokenByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var get = new Get()
            {
                UserId = userId
            };
            
            var requestWrapperGet = new UserApiGetCollectionRequestWrapper<Get, PasswordResetUserDto>(get, httpContextAccessor.HttpContext);
            var collection = await mediator.Send(requestWrapperGet, cancellationToken);
            var passwordResetUser = collection.Items.FirstOrDefault();

            if (passwordResetUser != null)
            {
                var delete = new Delete()
                {
                    Id = passwordResetUser.Id
                };
                var requestWrapperDelete = new UserApiRequestWrapper<Delete>(delete, httpContextAccessor.HttpContext);
                
                await mediator.Send(requestWrapperDelete, cancellationToken);
            }
        }

        private async Task<string> AddPasswordResetUser(Guid userId, CancellationToken cancellationToken)
        {
            var add = new Add()
            {
                UserId = userId,
                PasswordResetToken = Guid.NewGuid().ToString()
            };
            var request = new UserApiRequestWrapper<Add, PasswordResetUserDto>(add, httpContextAccessor.HttpContext);
            await mediator.Send(request, cancellationToken);

            return add.PasswordResetToken;
        }
        
        private async Task SendEmail(string email, string link, CancellationToken cancellationToken)
        {
            var sendEmailWithPasswordResetLinkMqCommand = new SendEmailWithPasswordResetLinkMq
            {
                Email = email,
                PasswordResetTokenLink = link
            };
            
            await sendEmailWithPasswordResetLinkPublisher.Produce(sendEmailWithPasswordResetLinkMqCommand, cancellationToken: cancellationToken);
        }
    }
}