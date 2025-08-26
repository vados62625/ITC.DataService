using Swashbuckle.AspNetCore.SwaggerUI;

namespace ITC.Authorization.Extensions.SwaggerFilters;

public static class SwaggerUiOptionsExtentions
{
    public static SwaggerUIOptions SwaggerEndpointVersion(this SwaggerUIOptions self, string version)
    {
        self.SwaggerEndpoint($"/swagger/{version}/swagger.json",
            $"API ver. {version}");
        return self;
    }

    public static SwaggerUIOptions SwaggerEndpointVersion(this SwaggerUIOptions self, IEnumerable<string> versions)
    {
        foreach (var version in versions)
        {
            self.SwaggerEndpointVersion(version);
        }
        return self;
    }
}