using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class WellKnownProviders
    {
        [JsonProperty("openid-configuration")]
        public HasOrder OpenIdConfiguration { get; set; }

        [JsonProperty("uma2-configuration")]
        public HasOrder Uma2Configuration { get; set; }
    }
}