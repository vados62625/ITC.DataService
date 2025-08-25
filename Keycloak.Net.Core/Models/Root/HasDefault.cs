using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class HasDefault
    {
        [JsonProperty("default")]
        public HasOrder Default { get; set; }
    }
}