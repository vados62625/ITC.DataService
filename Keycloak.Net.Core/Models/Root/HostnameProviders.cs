using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class HostnameProviders
    {
        [JsonProperty("request")]
        public HasOrder Request { get; set; }
    }
}