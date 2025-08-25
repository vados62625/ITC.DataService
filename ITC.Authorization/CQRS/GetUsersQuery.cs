using ITC.Authorization.Options;
using ITC.CQRS.Base.Get;
using ITC.Domain.Dto;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class GetUsersQuery : GetCollectionQueryBase, IRequest<PageableCollection<User>>
{

    /// <summary>
    /// Username, Firstname, Lastname, email
    /// </summary>
    public string? Search { get; set; }
    public string? Role { get; set; }
    public class Handler : IRequestHandler<GetUsersQuery, PageableCollection<User>>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _keycloakOptions;

        public Handler(KeycloakClient keycloak, IOptions<KeycloakClientOptions> keycloakOptions)
        {
            _keycloak = keycloak;
            _keycloakOptions = keycloakOptions.Value;
        }

        public async Task<PageableCollection<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await GetUsers(request, cancellationToken: cancellationToken);

            if (request.Id.HasValue)
            {
                users = users.Where(c => c.Id == request.Id);
                return new PageableCollection<User>(users.ToArray(), 1);
            }

            if (request is { Page: > 0, ItemsOnPage: > 0 })
            {
                users = users
                    .Skip(request.ItemsOnPage.Value * (request.Page.Value - 1))
                    .Take(request.ItemsOnPage.Value);

            }
            var usersArray = users.ToArray();

            return new PageableCollection<User>(usersArray, usersArray.Length);
        }

        private async Task<IEnumerable<User>> GetUsers(GetUsersQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Role))
            {
                var users = await _keycloak
                    .GetUsersWithRoleNameAsync(_keycloakOptions.Realm,
                    _keycloakOptions.ClientId.ToString(), request.Role, cancellationToken: cancellationToken);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    var search = request.Search.ToLower();
                    var usersArr = users.ToArray();
                    var usersQuery = usersArr.Where(c => c.UserName.ToLower().StartsWith(search));
                    var firstNameQuery = usersArr.Where(c => c.FirstName.ToLower().StartsWith(search));
                    var lastNameQuery = usersArr.Where(c => c.LastName.ToLower().StartsWith(search));
                    var emailQuery = usersArr.Where(c => c.Email.ToLower().StartsWith(search));

                    users = usersQuery
                        .Union(firstNameQuery)
                        .Union(lastNameQuery)
                        .Union(emailQuery);
                }

                return users;
            }
            else
            {
                return await _keycloak.GetUsersAsync(
                    _keycloakOptions.Realm,
                    search: request.Search,
                    cancellationToken: cancellationToken);
            }
        }
    }
}