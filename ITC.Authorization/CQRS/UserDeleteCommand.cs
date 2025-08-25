using ITC.Authorization.Options;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class UserDeleteCommand : IRequest<bool>
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<UserDeleteCommand, bool>
    {
        private readonly KeycloakClient _keycloakClient;
        private readonly KeycloakClientOptions _keycloakOptions;

        public Handler(IMediator mediator, KeycloakClient keycloakClient, IOptions<KeycloakClientOptions> keycloakOptions)
        {
            _keycloakClient = keycloakClient;
            _keycloakOptions = keycloakOptions.Value;
        }
        public async Task<bool> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            var isSuccess = await _keycloakClient.DeleteUserAsync(_keycloakOptions.Realm, request.UserId.ToString(), cancellationToken);

            Log.Info("User with id {0} was {1} updated", request.UserId, isSuccess ? "successfully" : "not");
            
            return isSuccess;
        }
    }
}