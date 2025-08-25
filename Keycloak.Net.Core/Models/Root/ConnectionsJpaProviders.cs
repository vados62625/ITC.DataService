using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class ConnectionsJpaProviders
    {
        [JsonProperty("default")]
        public Default Default { get; set; }
    }
}