using ITC.CQRS.Extensions;

namespace ITC.Authorization.Extensions;

public static class UserContextExtensions
{
    public static bool IsAdmin(this UserContext user)
    {
        return user.Roles.Contains("admin");
    }
}