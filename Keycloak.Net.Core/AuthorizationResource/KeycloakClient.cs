using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        public async Task<bool> CreateResourceAsync(string realm, string resourceServerId, Core.Models.AuthorizationResources.AuthorizationResource resource, CancellationToken cancellationToken = default)
        {
            var response =  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/clients/{resourceServerId}/authz/resource-server/resource")
                .PostJsonAsync(resource, cancellationToken)
                .ConfigureAwait(false);
            return response.ResponseMessage.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Core.Models.AuthorizationResources.AuthorizationResource>> GetResourcesAsync(string realm, string resourceServerId = null,
            bool deep = false, int? first = null, int? max = null, string name = null, string owner = null,
            string type = null, string uri = null, CancellationToken cancellationToken = default)
        {
            var queryParams = new Dictionary<string, object>
            {
                [nameof(deep)] = deep,
                [nameof(first)] = first,
                [nameof(max)] = max,
                [nameof(name)] = name,
                [nameof(owner)] = owner,
                [nameof(type)] = type,
                [nameof(uri)] = uri
            };

            return  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/clients/{resourceServerId}/authz/resource-server/resource")
                .SetQueryParams(queryParams)
                .GetJsonAsync<IEnumerable<Core.Models.AuthorizationResources.AuthorizationResource>>(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Core.Models.AuthorizationResources.AuthorizationResource> GetResourceAsync(string realm, string resourceServerId, string resourceId, CancellationToken cancellationToken = default) =>  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/clients/{resourceServerId}/authz/resource-server/resource/{resourceId}")
            .GetJsonAsync<Core.Models.AuthorizationResources.AuthorizationResource>(cancellationToken)
            .ConfigureAwait(false);

        public async Task<bool> UpdateResourceAsync(string realm, string resourceServerId, string resourceId, Core.Models.AuthorizationResources.AuthorizationResource resource, CancellationToken cancellationToken = default)
        {
            var response =  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/clients/{resourceServerId}/authz/resource-server/resource/{resourceId}")
                .PutJsonAsync(resource, cancellationToken)
                .ConfigureAwait(false);
            return response.ResponseMessage.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteResourceAsync(string realm, string resourceServerId, string resourceId, CancellationToken cancellationToken = default)
        {
            var response =  await UrlBuilderExtensions.AppendPathSegment((await GetBaseUrl(realm)), $"/admin/realms/{realm}/clients/{resourceServerId}/authz/resource-server/resource/{resourceId}")
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            return response.ResponseMessage.IsSuccessStatusCode;
        }
    }
}