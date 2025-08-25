using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class AccountProviders
    {
        [JsonProperty("freemarker")]
        public HasOrder Freemarker { get; set; }
    }
}