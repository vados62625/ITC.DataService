using System.ComponentModel.DataAnnotations;
using System.Net;
using Flurl.Http;
using ITC.Authorization.Options;
using Keycloak.Net;
using Keycloak.Net.Core.Models.AccessToken;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class RefreshCommand : IRequest<AccessToken?>
{
    [Required] public string RefreshToken { get; set; } = "";

    public class Handler : IRequestHandler<RefreshCommand, AccessToken?>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;

        public Handler(KeycloakClient keycloak, IOptions<KeycloakClientOptions> options)
        {
            _keycloak = keycloak;
            _clientOptions = options.Value;
        }

        public async Task<AccessToken?> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _keycloak.RefreshAccessToken(_clientOptions.Realm, request.RefreshToken, cancellationToken);
                return token;
            }
            catch (FlurlHttpException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.BadRequest)
                    return null;
                throw;
            }

        }
    }
}