using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class TruststoreProviders
    {
        [JsonProperty("file")]
        public HasOrder File { get; set; }
    }
}