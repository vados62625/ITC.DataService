using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class TimerProviders
    {
        [JsonProperty("basic")]
        public HasOrder Basic { get; set; }
    }
}