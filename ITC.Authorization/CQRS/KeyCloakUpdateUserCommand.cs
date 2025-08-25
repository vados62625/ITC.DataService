using FluentValidation;
using ITC.Authorization.Options;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class KeyCloakUpdateUserCommand : IRequest<User>
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public required User User { get; set; }

    public class Handler : IRequestHandler<KeyCloakUpdateUserCommand, User>
    {
        private readonly KeycloakClient _keycloakClient;
        private readonly KeycloakClientOptions _keycloakOptions;

        public Handler(KeycloakClient keycloakClient, IOptions<KeycloakClientOptions> keycloakOptions)
        {
            _keycloakClient = keycloakClient;
            _keycloakOptions = keycloakOptions.Value;
        }
        public async Task<User> Handle(KeyCloakUpdateUserCommand request, CancellationToken cancellationToken)
        {

            var user = await _keycloakClient.GetUserAsync(_keycloakOptions.Realm, request.User.Id, cancellationToken);
            if (user == null)
            {
                Log.Warn("User {0} not found in realm {1}", request.User.Id, _keycloakOptions.Realm);
                throw new ValidationException("Пользователь не найден");
            }

            var isSuccess = await _keycloakClient.UpdateUserAsync(_keycloakOptions.Realm, user.Id, request.User, cancellationToken);

            Log.Info("User with id {0} was {1} updated", request.User.Id, isSuccess ? "successfully" : "not");

            user = await _keycloakClient.GetUserAsync(_keycloakOptions.Realm, request.User.Id, cancellationToken);
            return user;
        }
    }
}