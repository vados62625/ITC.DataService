using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class BruteForceProtector
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public BruteForceProtectorProviders Providers { get; set; }
    }
}