using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class ScriptingProviders
    {
        [JsonProperty("script-based-auth")]
        public HasOrder ScriptBasedAuth { get; set; }
    }
}