using FluentValidation;
using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class CheckEmailVerificationCode : IRequest
{
    public required string Email { get; set; }
    public required string VerificationCode { get; set; }

    public class Handler(
        KeycloakClient keycloak,
        IOptions<KeycloakClientOptions> options,
        IServiceBusMessagePublisher<UserEmailConfirmedMq> sendEmployeeEmailConfirmedMqPublisher)
        : IRequestHandler<CheckEmailVerificationCode>
    {
        private readonly KeycloakClientOptions _clientOptions = options.Value;

        public async Task Handle(CheckEmailVerificationCode request, CancellationToken cancellationToken)
        {
            var query = $"emailverificationcode:{request.VerificationCode}";
            var users = await keycloak.GetUsersAsync(_clientOptions.Realm, email: request.Email, q: query, cancellationToken: cancellationToken);
            var user = users.FirstOrDefault();
            if(user == null)
                throw new ValidationException("Неверный код подтверждения");

            if(user.EmailVerified == true)
                throw new ValidationException("Почта уже подтверждена");

            user.EmailVerified = true;
            await keycloak.UpdateUserAsync(_clientOptions.Realm, user.Id, user, cancellationToken: cancellationToken);
            await sendEmployeeEmailConfirmedMqPublisher.Produce(
                new UserEmailConfirmedMq(user.Email), cancellationToken: cancellationToken);
        }
    }
}