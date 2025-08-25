using ITC.Authorization.Options;
using ITC.Authorization.Services;
using ITC.CQRS.Base;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class UserSetRolesCommand : IRequest
{
    public required string[] Roles { get; set; }
    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<UserApiRequestWrapper<UserSetRolesCommand>>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;
        private readonly IUserRolesCacheService _userRolesCacheService;

        public Handler(KeycloakClient keycloak, IOptions<KeycloakClientOptions> options,
            IUserRolesCacheService userRolesCacheService)
        {
            _keycloak = keycloak;
            _clientOptions = options.Value;
            _userRolesCacheService = userRolesCacheService;
        }

        public async Task Handle(UserApiRequestWrapper<UserSetRolesCommand> request,
            CancellationToken cancellationToken)
        {
            var rolesFromKeycloak = await _keycloak.GetRolesAsync(_clientOptions.Realm,
                _clientOptions.ClientId.ToString(), cancellationToken: cancellationToken);
            var roles = rolesFromKeycloak.Where(c => request.Request.Roles.Contains(c.Name)).ToArray();

            var oldRoles = (await _keycloak.GetClientRoleMappingsForUserAsync(_clientOptions.Realm,
                request.Request.UserId.ToString(),
                _clientOptions.ClientId.ToString(),
                cancellationToken)).ToArray();

            var rolesToRemove = oldRoles.Except(roles).ToArray();
            var rolesToAdd = roles.Except(oldRoles).ToArray();

            var isSuccess = await _keycloak.DeleteClientRoleMappingsFromUserAsync(_clientOptions.Realm,
                                request.Request.UserId.ToString(),
                                _clientOptions.ClientId.ToString(),
                                rolesToRemove,
                                cancellationToken)
                            && await _keycloak.AddClientRoleMappingsToUserAsync(_clientOptions.Realm,
                                request.Request.UserId.ToString(),
                                _clientOptions.ClientId.ToString(),
                                rolesToAdd,
                                cancellationToken);
            if (isSuccess)
            {
                await _userRolesCacheService.ProcessUserRoles(request.Request.UserId, cancellationToken);
            }
        }
    }
}