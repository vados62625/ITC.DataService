using Keycloak.Net.Core.Common.Converters;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    [JsonConverter(typeof(GroupNameConverter))]
    public enum GroupName
    {
        Social,
        UserDefined
    }
}