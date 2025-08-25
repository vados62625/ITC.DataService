using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class AuthorizationPersisterProviders
    {
        [JsonProperty("jpa")]
        public HasOrder Jpa { get; set; }
    }
}