using ITC.ReportService.CQRS.Engine;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ITC.ReportService.Hub;

public class EngineHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IMediator _mediator;
    public const string NewEngineNotificationMethodName = "engineUpdate";
    
    public EngineHub(IMediator mediator)
    {
        _mediator = mediator;
    }
}