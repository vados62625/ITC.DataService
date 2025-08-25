using Keycloak.Net.Core.Common.Converters;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    [JsonConverter(typeof(NameConverter))]
    public enum Name
    {
        Base,
        Keycloak,
        RhSso
    }
}