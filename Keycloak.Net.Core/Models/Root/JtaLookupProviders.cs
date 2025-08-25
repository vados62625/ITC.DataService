using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class JtaLookupProviders
    {
        [JsonProperty("jboss")]
        public HasOrder Jboss { get; set; }
    }
}