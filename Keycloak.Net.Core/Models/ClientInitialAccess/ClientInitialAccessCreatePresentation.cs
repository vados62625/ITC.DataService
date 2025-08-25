using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.ClientInitialAccess
{
    public class ClientInitialAccessCreatePresentation
    {
        [JsonProperty("count")]
        public int? Count { get; set; }
        [JsonProperty("expiration")]
        public int? Expiration { get; set; }
    }
}