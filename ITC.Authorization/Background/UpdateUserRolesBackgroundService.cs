using ITC.Authorization.Options;
using ITC.Authorization.Services;
using ITC.Authorization.Storage;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.Background;

public class UpdateUserRolesBackgroundService : BackgroundServiceBase
{
    private readonly KeycloakClientOptions _keycloakOptions;
    public UpdateUserRolesBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BackgroundServiceBase> logger,
        IOptions<KeycloakClientOptions> keycloakOptions
    ) : base(serviceScopeFactory, logger)
    {
        _keycloakOptions = keycloakOptions.Value;
    }

    protected override TimeSpan Period => TimeSpan.FromSeconds(10);

    protected override async Task Work(CancellationToken cancellationToken)
    {
        using var scope = _factory.CreateScope();

        var keycloak = scope.ServiceProvider.GetRequiredService<KeycloakClient>();
        var db = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
        var userRoleCacheService = scope.ServiceProvider.GetRequiredService<IUserRolesCacheService>();

        var usersFromKeycloak = await keycloak.GetUsersAsync(_keycloakOptions.Realm, max: -1, cancellationToken: cancellationToken);

        var cachedUserRoles = await db.Set<UserRolesCache>().ToListAsync(cancellationToken);

        foreach (var user in usersFromKeycloak)
        {
            await userRoleCacheService.ProcessUserRoles(user, cachedUserRoles, cancellationToken);
        }

        await RemoveDeletedUsersFromCache(db, cachedUserRoles, usersFromKeycloak, cancellationToken);
    }

    private async Task RemoveDeletedUsersFromCache(
        ServiceDbContext db,
        List<UserRolesCache> cachedUserRoles,
        IEnumerable<User> usersFromKeycloak,
        CancellationToken cancellationToken)
    {
        var keycloakUserIds = usersFromKeycloak.Select(user => user.GetEmployeeId()).Where(user => user != null).ToHashSet();

        var usersToRemove = cachedUserRoles
            .Where(cachedUser => !keycloakUserIds.Contains(cachedUser.UserId))
            .ToList();

        if (!usersToRemove.Any())
            return;

        db.Set<UserRolesCache>().RemoveRange(usersToRemove);
        await db.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Removed {usersToRemove.Count} users from cache.");
    }
}