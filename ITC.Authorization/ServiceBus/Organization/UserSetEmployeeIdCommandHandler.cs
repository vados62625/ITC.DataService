using ITC.Authorization.CQRS;
using ITC.ServiceBus.Interfaces;
using MediatR;

namespace ITC.Authorization.ServiceBus.Organization;

public class UserSetEmployeeIdCommandHandler : IServiceBusMessageHandler<UserSetEmployeeIdCommand>
{
    private readonly IMediator _mediator;

    public UserSetEmployeeIdCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(UserSetEmployeeIdCommand message, IDictionary<string, string> headers, DateTimeOffset timestamp,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(message, cancellationToken);
    }
}