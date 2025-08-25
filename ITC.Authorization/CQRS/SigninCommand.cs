using System.ComponentModel.DataAnnotations;
using System.Net;
using Flurl.Http;
using ITC.Authorization.Options;
using Keycloak.Net;
using Keycloak.Net.Core.Models.AccessToken;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class SigninCommand : IRequest<AccessToken?>
{
    [Required] public string UserId { get; set; } = "";
    [Required] public string Password { get; set; } = "";

    public class Handler : IRequestHandler<SigninCommand, AccessToken?>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;

        public Handler(KeycloakClient keycloak, IOptions<KeycloakClientOptions> options)
        {
            _keycloak = keycloak;
            _clientOptions = options.Value;
        }

        public async Task<AccessToken?> Handle(SigninCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _keycloak.GetAccessToken(_clientOptions.Realm, request.UserId, request.Password,
                    cancellationToken);
                return token;
            }
            catch (FlurlHttpException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.Unauthorized)
                    return null;
                throw;
            }
        }
    }
}