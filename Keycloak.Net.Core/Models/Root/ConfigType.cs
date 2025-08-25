using Keycloak.Net.Core.Common.Converters;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    [JsonConverter(typeof(ConfigTypeConverter))]
    public enum ConfigType
    {
        Int,
        String
    }
}