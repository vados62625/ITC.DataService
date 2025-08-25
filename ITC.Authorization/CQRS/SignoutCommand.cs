using ITC.Authorization.Options;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class SignoutCommand : IRequest
{
    public Guid UserId { get; }

    public SignoutCommand(Guid userId)
    {
        UserId = userId;
    }

    public class Handler : IRequestHandler<SignoutCommand>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;

        public Handler(KeycloakClient keycloak, IOptions<KeycloakClientOptions> options)
        {
            _keycloak = keycloak;
            _clientOptions = options.Value;
        }

        public async Task Handle(SignoutCommand request, CancellationToken cancellationToken)
        {
            await _keycloak.RemoveUserSessionsAsync(_clientOptions.Realm, request.UserId, cancellationToken);
        }
    }
}