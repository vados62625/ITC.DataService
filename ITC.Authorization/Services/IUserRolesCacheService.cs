using ITC.Authorization.Storage;
using Keycloak.Net.Core.Models.Users;

namespace ITC.Authorization.Services;

public interface IUserRolesCacheService
{
    Task ProcessUserRoles(
        Guid userId,
        CancellationToken cancellationToken);

    Task ProcessUserRoles(
        User user,
        List<UserRolesCache> cachedUserRoles,
        CancellationToken cancellationToken);
}