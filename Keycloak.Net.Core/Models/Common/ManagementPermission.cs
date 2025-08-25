using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Common
{
    public class ManagementPermission
    {
        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }
    }
}