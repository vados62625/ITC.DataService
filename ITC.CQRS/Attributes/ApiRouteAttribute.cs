using Microsoft.AspNetCore.Mvc;

namespace ITC.CQRS.Attributes;

public class ApiRouteAttribute : RouteAttribute
{
    /// <summary>
    /// api/v{version:apiVersion}/[controller]
    /// </summary>
    public ApiRouteAttribute() : base("api/v{version:apiVersion}/[controller]")
    {
    }
    
    /// <summary>
    /// api/v{{version:apiVersion}}/{route}/[controller]
    /// </summary>
    /// <param name="route"></param>
    // ReSharper disable once RouteTemplates.SyntaxError
    public ApiRouteAttribute(string route) : base($"api/v{{version:apiVersion}}/{route}/[controller]")
    {
    }
}