using Keycloak.Net.Core.Common.Converters;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    [JsonConverter(typeof(ProtocolConverter))]
    public enum Protocol
    {
        DockerV2,
        OpenIdConnect,
        Saml
    }
}