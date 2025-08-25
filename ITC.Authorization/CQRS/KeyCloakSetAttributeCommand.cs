using FluentValidation;
using ITC.Authorization.Options;
using Keycloak.Net;
using Keycloak.Net.Core.Models.Users;
using MediatR;
using Microsoft.Extensions.Options;
using NLog;
using ILogger = NLog.ILogger;

namespace ITC.Authorization.CQRS;

public class KeyCloakSetAttributeCommand : IRequest<User>
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public Guid UserId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }

    public class Handler : IRequestHandler<KeyCloakSetAttributeCommand, User>
    {
        private readonly KeycloakClient _keycloakClient;
        private readonly KeycloakClientOptions _keycloakOptions;

        public Handler(KeycloakClient keycloakClient, IOptions<KeycloakClientOptions> keycloakOptions)
        {
            _keycloakClient = keycloakClient;
            _keycloakOptions = keycloakOptions.Value;
        }
        public async Task<User> Handle(KeyCloakSetAttributeCommand request, CancellationToken cancellationToken)
        {

            var user = await _keycloakClient.GetUserAsync(_keycloakOptions.Realm, request.UserId, cancellationToken);
            if (user == null)
            {
                Log.Warn("User {0} not found in realm {1}", request.UserId, _keycloakOptions.Realm);
                throw new ValidationException("Пользователь не найден");
            }

            user.Attributes ??= new Dictionary<string, IEnumerable<string>>();

            user.Attributes[request.Key] = new[] { request.Value };

            var isSuccess = await _keycloakClient.UpdateUserAsync(_keycloakOptions.Realm, user.Id, user, cancellationToken);

            Log.Info("Set attribute {0} [{1}] for userid {2} issuccess {3}", request.Key, request.Value, request.UserId, isSuccess);

            user = await _keycloakClient.GetUserAsync(_keycloakOptions.Realm, request.UserId, cancellationToken);
            return user;
        }
    }
}