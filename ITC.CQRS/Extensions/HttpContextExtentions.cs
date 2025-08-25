using System.Security.Claims;

namespace ITC.CQRS.Extensions;

public static class HttpContextExtentions
{
    public static string EmployeeId = "employeeid";
    public static Guid GetUserId(this ClaimsPrincipal userClaimsPrincipal)
    {
        Guid.TryParse(userClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userGuid);
        return userGuid;
    }

    public static string GetUserLogin(this ClaimsPrincipal userClaimsPrincipal)
    {
        return userClaimsPrincipal.Identity?.Name ?? "";
    }
    public static string GetUserEmail(this ClaimsPrincipal userClaimsPrincipal)
    {
        return userClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? ""; ;
    }

    public static string GetUserName(this ClaimsPrincipal userClaimsPrincipal)
    {
        return userClaimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "";
    }

    public static string[] GetUserRoles(this ClaimsPrincipal userClaimsPrincipal)
    {
        return userClaimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
    }

    public static Guid GetEmployeeId(this ClaimsPrincipal userClaimsPrincipal)
    {
        Guid.TryParse(userClaimsPrincipal.FindFirst(EmployeeId)?.Value, out var employeeId);
        return employeeId;
    }
    public static UserContext GetUserContext(this ClaimsPrincipal userClaimsPrincipal)
    {
        return new UserContext(userClaimsPrincipal);
    }

  
}
public class UserContext
{
    public string Login { get; }
    public string Name { get; }
    public string Email { get; }
    public string[] Roles { get; }
    public Guid Id { get; }
    public Guid? EmployeeId { get; }
    public UserContext(ClaimsPrincipal userClaimsPrincipal)
    {
        Login = userClaimsPrincipal.GetUserLogin();
        Roles = userClaimsPrincipal.GetUserRoles();
        Id = userClaimsPrincipal.GetUserId();
        Email = userClaimsPrincipal.GetUserEmail();
        Name = userClaimsPrincipal.GetUserName();
        EmployeeId = userClaimsPrincipal.GetEmployeeId();
    }
}