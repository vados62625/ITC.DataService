using System.ComponentModel.DataAnnotations;
using ITC.Authorization.Options;
using ITC.CQRS.Base;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class ChangeUserEmail : IRequest 
{
    [Required] public string Email { get; set; }

    public class Handler(
        KeycloakClient keycloakClient,
        IOptions<KeycloakClientOptions> keycloakOptions
        ) : IRequestHandler<UserApiRequestWrapper<ChangeUserEmail>>
    {
        private readonly KeycloakClientOptions _keycloakOptions = keycloakOptions.Value;
        
        public async Task Handle(UserApiRequestWrapper<ChangeUserEmail> wrapper, CancellationToken cancellationToken)
        {
            var user = await keycloakClient.GetUserAsync(
                _keycloakOptions.Realm, 
                wrapper.UserContext.Id, 
                cancellationToken);
            
            if(user == null)
                return;
            
            user.Email = wrapper.Request.Email;
            await keycloakClient.UpdateUserAsync(
                _keycloakOptions.Realm, 
                wrapper.UserContext.Id, 
                user, 
                cancellationToken);
        }
    }
}