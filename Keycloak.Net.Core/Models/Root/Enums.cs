using System.Collections.Generic;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    public class Enums
    {
        [JsonProperty("operationType")]
        public List<string> OperationType { get; set; }

        [JsonProperty("eventType")]
        public List<string> EventType { get; set; }

        [JsonProperty("resourceType")]
        public List<string> ResourceType { get; set; }
    }
}