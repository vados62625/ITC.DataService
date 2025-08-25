using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class IdentityProvider
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public IdentityProviderProviders Providers { get; set; }
    }
}