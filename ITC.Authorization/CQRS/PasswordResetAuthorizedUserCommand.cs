using System.ComponentModel.DataAnnotations;
using ITC.Authorization.Options;
using ITC.CQRS.Base;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class PasswordResetAuthorizedUserCommand : IRequest
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    [Required] public string Password { get; set; }
    
    public class Handler(
        KeycloakClient keycloakClient,
        IOptions<KeycloakClientOptions> keycloakOptions,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<UserApiRequestWrapper<PasswordResetAuthorizedUserCommand>>
    {
        private readonly KeycloakClientOptions _keycloakOptions = keycloakOptions.Value;

        public async Task Handle(UserApiRequestWrapper<PasswordResetAuthorizedUserCommand> wrapper, CancellationToken cancellationToken)
        {
            var result = await keycloakClient.SetUserPasswordAsync(_keycloakOptions.Realm, wrapper.UserContext.Id.ToString(), wrapper.Request.Password, cancellationToken);
            
            if (!result.Success)
            {
                Log.Warn("Error when changing the password {0}", result.Error);
                throw new ValidationException("Ошибка при смене пароля");
            }
        }
    }
}