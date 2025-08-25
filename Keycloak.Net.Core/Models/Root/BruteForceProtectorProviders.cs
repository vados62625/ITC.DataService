using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class BruteForceProtectorProviders
    {
        [JsonProperty("default-brute-force-detector")]
        public HasOrder DefaultBruteForceDetector { get; set; }
    }
}