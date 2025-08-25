using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Keycloak.Net.Core.Models.Components;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        public async Task<IEnumerable<ComponentType>> GetRetrieveProvidersBasePathAsync(string realm, CancellationToken cancellationToken = default) =>  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/client-registration-policy/providers")
            .GetJsonAsync<IEnumerable<ComponentType>>(cancellationToken)
            .ConfigureAwait(false);
    }
}