using System.ComponentModel.DataAnnotations;
using ITC.Authorization.CQRS.PasswordResetUsers;
using ITC.Authorization.CQRS.PasswordResetUsers.Dto;
using ITC.Authorization.Options;
using ITC.CQRS.Base;
using ITC.CQRS.Base.Get;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class PasswordResetUserCommand : IRequest
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    [Required] public string Password { get; set; }
    [Required] public string PasswordResetToken { get; set; }
    
    public class Handler(
        KeycloakClient keycloakClient,
        IOptions<KeycloakClientOptions> keycloakOptions,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<PasswordResetUserCommand>
    {
        private readonly KeycloakClientOptions _keycloakOptions = keycloakOptions.Value;

        public async Task Handle(PasswordResetUserCommand request, CancellationToken cancellationToken)
        {
            var get = new Get()
            {
                PasswordResetToken = request.PasswordResetToken
            };
            
            var requestWrapper = new UserApiGetCollectionRequestWrapper<Get, PasswordResetUserDto>(get, httpContextAccessor.HttpContext);
            var test = await mediator.Send(requestWrapper, cancellationToken);

            var passwordResetUser = test.Items.FirstOrDefault();

            if (passwordResetUser == null)
                throw new ValidationException("Ссылка для смены пароля уже использована");
            
            var result = await keycloakClient.SetUserPasswordAsync(_keycloakOptions.Realm, passwordResetUser.UserId.ToString(), request.Password, cancellationToken);
            
            if (result.Success)
            {
                var delete = new Delete()
                {
                    Id = passwordResetUser.Id
                };
                var requestWrapperDelete = new UserApiRequestWrapper<Delete>(delete, httpContextAccessor.HttpContext);
                
                await mediator.Send(requestWrapperDelete, cancellationToken);
            }
            else 
            {
                Log.Warn("Error when changing the password {0}", result.Error);
                throw new ValidationException("Ошибка при смене пароля");
            }
        }
    }
}