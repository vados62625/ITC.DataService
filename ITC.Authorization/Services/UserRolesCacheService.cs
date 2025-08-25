using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus.Organization;
using ITC.Authorization.Storage;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.Services;

public class UserRolesCacheService : IUserRolesCacheService
{
    private readonly KeycloakClientOptions _keycloakOptions;
    private readonly IServiceBusMessagePublisher<UpdateUserRolesMq> _mqPublisher;
    private readonly ServiceDbContext _db;
    private readonly KeycloakClient _keycloak;
    public UserRolesCacheService(
        IOptions<KeycloakClientOptions> keycloakOptions,
        IServiceBusMessagePublisher<UpdateUserRolesMq> mqPublisher,
        ServiceDbContext db,
        KeycloakClient keycloak
    ) {
        _keycloakOptions = keycloakOptions.Value;
        _mqPublisher = mqPublisher;
        _db = db;
        _keycloak = keycloak;
    }

    public async Task ProcessUserRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _keycloak.GetUserAsync(
            _keycloakOptions.Realm,
            userId,
            cancellationToken);
        if (user == null)
            return;

        var cachedUserRoles = await _db.Set<UserRolesCache>().ToListAsync(cancellationToken);

        await ProcessUserRoles(user, cachedUserRoles, cancellationToken);
    }

    public async Task ProcessUserRoles(
        User user,
        List<UserRolesCache> cachedUserRoles,
        CancellationToken cancellationToken)
    {
        var employeeId = user.GetEmployeeId();
        if (!employeeId.HasValue)
            return;

        var userRolesFromKeycloak = await _keycloak.GetClientRoleMappingsForUserAsync(
            _keycloakOptions.Realm,
            user.Id.ToString(),
            _keycloakOptions.ClientId.ToString(),
            cancellationToken
        );
        var rolesFromKeycloak = userRolesFromKeycloak.Select(role => role.Name).ToArray();

        var cachedUserRole = cachedUserRoles.FirstOrDefault(cachedUser => cachedUser.UserId == employeeId.Value);
        if (cachedUserRole == null)
        {
            var newUserRolesCache = new UserRolesCache
            {
                UserId = employeeId.Value,
                Roles = rolesFromKeycloak
            };
            newUserRolesCache.Roles = rolesFromKeycloak;
            cachedUserRoles.Add(newUserRolesCache);

            _db.Set<UserRolesCache>().Add(newUserRolesCache);
            await _db.SaveChangesAsync(cancellationToken);

            await SendToMq(user, rolesFromKeycloak);
            return;
        }

        // Если пользователь есть, проверяем изменения
        if (!HasRolesChanged(cachedUserRole.Roles, rolesFromKeycloak))
            return;

        // Если изменения есть, обновляем кэш и отправляем сообщение в очередь
        cachedUserRole.Roles = rolesFromKeycloak;
        _db.Set<UserRolesCache>().Update(cachedUserRole);
        await _db.SaveChangesAsync(cancellationToken);

        await SendToMq(user, rolesFromKeycloak);
    }

    private async Task SendToMq(User user, string[] roles)
    {
        var employeeId = user.GetEmployeeId();
        if (employeeId == null)
            return;

        var message = new UpdateUserRolesMq
        {
            UserId = employeeId.Value,
            Roles = roles
        };

        await _mqPublisher.Produce(message);
    }

    private bool HasRolesChanged(string[] cachedRoles, string[] newRoles)
    {
        // Сравниваем роли: если наборы совпадают, изменений нет
        return !cachedRoles.All(newRoles.Contains) || cachedRoles.Length != newRoles.Length;
    }
}