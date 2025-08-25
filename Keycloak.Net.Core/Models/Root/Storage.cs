using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class Storage
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public StorageProviders Providers { get; set; }
    }
}