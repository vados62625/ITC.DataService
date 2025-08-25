using Flurl.Http;
using ITC.Authorization.Options;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class GetUserQuery : IRequest<User?>
{
    public GetUserQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public class Handler : IRequestHandler<GetUserQuery, User?>
    {
        private readonly KeycloakClient _keycloakClient;
        private readonly KeycloakClientOptions _keycloakOptions;

        public Handler(KeycloakClient keycloakClient, IOptions <KeycloakClientOptions> keycloakOptions)
        {
            _keycloakClient = keycloakClient;
            _keycloakOptions = keycloakOptions.Value;
        }

        public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _keycloakClient
                    .GetUserAsync(_keycloakOptions.Realm, request.Id, cancellationToken);
                return user;
            }
            catch (FlurlHttpException e)
            {
                if (e.StatusCode == 404)
                    return null;

                throw;
            }
        }
    }
}