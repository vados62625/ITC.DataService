using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        public async Task<Core.Models.AccessToken.AccessToken> GetAccessToken(string realm, string userId, string password, CancellationToken cancellationToken)
        {
            var url = new Uri(_url);
            var resp = await url
                .AppendPathSegment($"{_options.Prefix}/realms/{realm}/protocol/openid-connect/token")
                .WithHeader("Accept", "application/json")
                .PostUrlEncodedAsync(new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "password"),
                    new("username", userId),
                    new("password", password),
                    new("client_id", _options.AdminClientId),
                    new("client_secret", _clientSecret),
                    new("scope", "offline_access")
                }, cancellationToken: cancellationToken)
                .ReceiveJson<Core.Models.AccessToken.AccessToken>()
                .ConfigureAwait(false);
            return resp;
        }

        public async Task<Core.Models.AccessToken.AccessToken> RefreshAccessToken(string realm, string refreshToken, CancellationToken cancellationToken)
        {
            var url = new Uri(_url);
            var resp = await url
                .AppendPathSegment($"{_options.Prefix}/realms/{realm}/protocol/openid-connect/token")
                .WithHeader("Accept", "application/json")
                .PostUrlEncodedAsync(new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "refresh_token"),
                    new("refresh_token", refreshToken),
                    new("client_id", _options.AdminClientId),
                    new("client_secret", _clientSecret),
                    new("scope", "offline_access")
                }, cancellationToken: cancellationToken)
                .ReceiveJson<Core.Models.AccessToken.AccessToken>()
                .ConfigureAwait(false);
            return resp;
        }
    }
}