using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class AuthorizationPersister
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public AuthorizationPersisterProviders Providers { get; set; }
    }
}