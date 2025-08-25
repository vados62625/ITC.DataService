using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class HasOrder
    {
        [JsonProperty("order")]
        public long Order { get; set; }
    }
}