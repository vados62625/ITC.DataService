using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Keycloak.Net.Core.Models.Key;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        public async Task<KeysMetadata> GetKeysAsync(string realm, CancellationToken cancellationToken = default) =>  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/keys")
            .GetJsonAsync<KeysMetadata>(cancellationToken)
            .ConfigureAwait(false);
    }
}