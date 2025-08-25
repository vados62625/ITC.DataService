using Keycloak.Net.Core.Common.Converters;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    [JsonConverter(typeof(JsonTypeLabelConverter))]
    public enum JsonTypeLabel
    {
        Boolean,
        ClientList,
        File,
        List,
        MultivaluedList,
        MultivaluedString,
        Password,
        Role,
        Script,
        String,
        Text
    }
}