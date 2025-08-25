using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Keycloak.Net.Core.Models.AttackDetection;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        public async Task<bool> ClearUserLoginFailuresAsync(string realm, CancellationToken cancellationToken = default)
        {
            var response = await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/attack-detection/brute-force/users")
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            return response.ResponseMessage.IsSuccessStatusCode;
        }

        public async Task<bool> ClearUserLoginFailuresAsync(string realm, string userId, CancellationToken cancellationToken = default)
        {
            var response = await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/attack-detection/brute-force/users/{userId}")
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            return response.ResponseMessage.IsSuccessStatusCode;
        }

        public async Task<UserNameStatus> GetUserNameStatusInBruteForceDetectionAsync(string realm, string userId, CancellationToken cancellationToken = default) =>  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/attack-detection/brute-force/users/{userId}")
            .GetJsonAsync<UserNameStatus>(cancellationToken)
            .ConfigureAwait(false);
    }
}