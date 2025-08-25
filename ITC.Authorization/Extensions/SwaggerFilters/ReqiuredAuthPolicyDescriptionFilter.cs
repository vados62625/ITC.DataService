using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITC.Authorization.Extensions.SwaggerFilters;

public class ReqiuredAuthPolicyDescriptionFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attr = context.MethodInfo.GetCustomAttributes(true).FirstOrDefault(c => c is AuthorizeAttribute);

        const string policy = "Политика авторизации: ";

        if (attr == null)
        {
            operation.Description = policy + "[Отсутствует]";
            return;
        }

        var authorizeAttribute = (AuthorizeAttribute)attr;

        operation.Description = policy + $"[{authorizeAttribute.Policy}]";
    }
}