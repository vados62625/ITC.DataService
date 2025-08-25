using Keycloak.Net.Core.Common.Converters;
using Newtonsoft.Json;

namespace Keycloak.Net.Core.Models.Root
{
    [JsonConverter(typeof(CategoryConverter))]
    public enum Category
    {
        AttributeStatementMapper,
        DockerAuthMapper,
        GroupMapper,
        RoleMapper,
        TokenMapper
    }
}