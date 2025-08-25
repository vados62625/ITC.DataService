using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITC.Authorization.Extensions.SwaggerFilters;

public class RequestIdHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Request-id",
            In = ParameterLocation.Header,
            Description = "Root Trace Id",
            Required = false
        });
    }
}