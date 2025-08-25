using System.Collections.Generic;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.AuthorizationResources
{
    public class AuthorizationResource
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("scopes")]
        public IEnumerable<AuthorizationScopes.AuthorizationScope> Scopes { get; set; }
        [JsonProperty("attributes")]
        public Dictionary<string, IEnumerable<string>> Attributes { get; set; }
        [JsonProperty("uris")]
        public IEnumerable<string> Uris { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("ownerManagedAccess")]
        public bool? OwnerManagedAccess { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("owner")]
        public AuthorizationResourceOwner Owner { get; set; }
    }
}