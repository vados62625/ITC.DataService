using System.Collections.Generic;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Clients
{
    public class AccessTokenAccess
    {
        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }
        [JsonProperty("verify_caller")]
        public bool? VerifyCaller { get; set; }
    }
}