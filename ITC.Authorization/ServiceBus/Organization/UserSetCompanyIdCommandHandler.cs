using ITC.Authorization.CQRS;
using ITC.ServiceBus.Interfaces;
using MediatR;

namespace ITC.Authorization.ServiceBus.Organization;

public class UserSetCompanyIdCommandHandler : IServiceBusMessageHandler<UserSetCompanyIdCommand>
{
    private readonly IMediator _mediator;

    public UserSetCompanyIdCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(UserSetCompanyIdCommand message, IDictionary<string, string> headers, DateTimeOffset timestamp,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(message, cancellationToken);
    }
}