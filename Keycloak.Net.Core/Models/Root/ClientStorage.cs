using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class ClientStorage
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public MetadataClass Providers { get; set; }
    }
}