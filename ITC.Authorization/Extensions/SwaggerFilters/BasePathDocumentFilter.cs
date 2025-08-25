using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITC.Authorization.Extensions.SwaggerFilters;

public class BasePathDocumentFilter : IDocumentFilter
{
    private readonly string _basePath;
    public BasePathDocumentFilter(string basePath)
    {
        _basePath = basePath;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = _basePath }
        };
    }
} 