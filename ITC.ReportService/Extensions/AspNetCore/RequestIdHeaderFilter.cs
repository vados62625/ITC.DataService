using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITC.ReportService.Extensions.AspNetCore;

public class RequestIdHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Request-id",
            In = ParameterLocation.Header,
            Description = "Root request id",
            Required = false
        });
    }
}