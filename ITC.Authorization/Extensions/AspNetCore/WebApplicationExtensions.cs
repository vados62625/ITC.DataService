using ITC.Authorization.Extensions.SwaggerFilters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ITC.Authorization.Extensions.AspNetCore;

public static class WebApplicationExtensions
{
    // ReSharper disable once InconsistentNaming
    public static WebApplication UseVersionedSwaggerUI(this WebApplication webApplication,
        IEnumerable<string> controllersVersions)
    {
        webApplication
            .UseSwagger(c =>
            {
                c.RouteTemplate = webApplication.Environment.IsDevelopment()
                    ? "authorization/swagger/{documentName}/swagger.json"
                    : "swagger/{documentName}/swagger.json";
            })
            .UseSwaggerUI(options =>
            {
                options.ConfigObject = new ConfigObject
                {
                    ShowCommonExtensions = true

                };
                options.EnableDeepLinking();
                options.SwaggerEndpointVersion(controllersVersions);

            });
        return webApplication;
    }
}