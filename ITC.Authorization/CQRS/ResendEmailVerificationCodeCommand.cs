using FluentValidation;
using ITC.Authorization.Options;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.ServiceBus.Interfaces;
using Keycloak.Net;
using MediatR;
using Microsoft.Extensions.Options;

namespace ITC.Authorization.CQRS;

public class ResendEmailVerificationCodeCommand : IRequest
{
    public string Email { get; set; }

    public ResendEmailVerificationCodeCommand(string email)
    {
        Email = email;
    }

    public class Handler : IRequestHandler<ResendEmailVerificationCodeCommand>
    {
        private readonly KeycloakClient _keycloak;
        private readonly KeycloakClientOptions _clientOptions;
        private readonly IServiceBusMessagePublisher<SendEmailWithVerificationCodeMq> _sendEmailWithVerificationCodePublisher;

        public Handler(
            KeycloakClient keycloak,
            IOptions<KeycloakClientOptions> options,
            IServiceBusMessagePublisher<SendEmailWithVerificationCodeMq> sendEmailWithVerificationCodePublisher)
        {
            _keycloak = keycloak;
            _clientOptions = options.Value;
            _sendEmailWithVerificationCodePublisher = sendEmailWithVerificationCodePublisher;
        }

        public async Task Handle(ResendEmailVerificationCodeCommand command, CancellationToken cancellationToken)
        {
            var users = await _keycloak.GetUsersAsync(_clientOptions.Realm, email: command.Email, cancellationToken: cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
                throw new ValidationException($"Не найден пользовать с email:{command.Email}");

            var newVerificationCode = GenerateVerificationCode();

            if (user.Attributes.ContainsKey("emailverificationcode"))
            {
                user.Attributes["emailverificationcode"] = new List<string> { newVerificationCode.ToString() };
            }
            else
            {
                user.Attributes.Add("emailverificationcode", new List<string> { newVerificationCode.ToString() });
            }

            var isSuccess = await _keycloak.UpdateUserAsync(_clientOptions.Realm, user.Id, user, cancellationToken: cancellationToken);
            if (isSuccess)
            {
                await SendEmailWithVerificationCodeMq(command.Email, newVerificationCode, cancellationToken);
            }
        }

        private int GenerateVerificationCode()
        {
            return Random.Shared.Next(100000, 999999);
        }

        private async Task SendEmailWithVerificationCodeMq(string email, int verificationCode, CancellationToken cancellationToken)
        {
            var sendEmailWithVerificationCodeMqCommand = new SendEmailWithVerificationCodeMq
            {
                VerificationCode = verificationCode.ToString(),
                Email = email
            };

            await _sendEmailWithVerificationCodePublisher.Produce(sendEmailWithVerificationCodeMqCommand, cancellationToken: cancellationToken);
        }
    }
}