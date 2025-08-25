using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class AuthorizationCache
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public HasDefault Providers { get; set; }
    }
}