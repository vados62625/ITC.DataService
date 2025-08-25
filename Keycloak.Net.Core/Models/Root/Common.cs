using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class Common
    {
        [JsonProperty("name")]
        public Name Name { get; set; }
    }
}