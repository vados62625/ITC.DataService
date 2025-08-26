using ITC.Domain.Dto;
using ITC.ServiceBus.Interfaces;
using MediatR;

namespace ITC.ReportService.ServiceBus;

public class AnalysisResultHandler : IServiceBusMessageHandler<CsvDataResponse>
{
    private readonly IMediator _mediator;

    public AnalysisResultHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CsvDataResponse message, IDictionary<string, string> headers, DateTimeOffset timestamp,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(message, cancellationToken);
    }
}