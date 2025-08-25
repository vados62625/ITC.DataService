using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class KeycloakStorageUserStorageProviderMetadata
    {
        [JsonProperty("synchronizable")]
        public bool? Synchronizable { get; set; }
    }
}