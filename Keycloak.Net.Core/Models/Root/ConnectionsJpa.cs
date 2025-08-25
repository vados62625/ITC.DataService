using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class ConnectionsJpa
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public ConnectionsJpaProviders Providers { get; set; }
    }
}