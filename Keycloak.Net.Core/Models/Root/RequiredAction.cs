using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class RequiredAction
    {
        [JsonProperty("internal")]
        public bool? Internal { get; set; }

        [JsonProperty("providers")]
        public RequiredActionProviders Providers { get; set; }
    }
}