using ITC.Authorization.CQRS;
using ITC.ServiceBus.Interfaces;
using MediatR;

namespace ITC.Authorization.ServiceBus.Organization;

public class UserSetBrokerIdCommandHandler : IServiceBusMessageHandler<UserSetBrokerIdCommand>
{
    private readonly IMediator _mediator;

    public UserSetBrokerIdCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(UserSetBrokerIdCommand message, IDictionary<string, string> headers, DateTimeOffset timestamp,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(message, cancellationToken);
    }
}