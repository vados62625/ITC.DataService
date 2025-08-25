using Keycloak.Net.Core.Models.Users;
using MediatR;
using NLog;
using ILogger = NLog.ILogger;


namespace ITC.Authorization.CQRS;

public class UserSetBrokerIdCommand : IRequest<User>
{
    private const string AttributeKey = "brokerid";

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public Guid UserId { get; set; }
    public Guid BrokerId { get; set; }

    public class Handler : IRequestHandler<UserSetBrokerIdCommand, User>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<User> Handle(UserSetBrokerIdCommand request, CancellationToken cancellationToken)
        {
            var command = new KeyCloakSetAttributeCommand
            {
                UserId = request.UserId,
                Key = AttributeKey,
                Value = request.BrokerId.ToString()
            };

            return await _mediator.Send(command, cancellationToken);
        }
    }
}