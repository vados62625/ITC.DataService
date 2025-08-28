using System.Text.Json.Serialization;
using ITC.Domain.Enums;
using ITC.Domain.Models;
using ITC.ReportService.CQRS.Engine;
using ITC.ReportService.Database;
using ITC.ReportService.Hub;
using ITC.ReportService.Services;
using ITC.ServiceBus.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.ServiceBus;

public class CsvDataResponse
{
    [JsonPropertyName("defects")] public Dictionary<string, double> Defects { get; set; } = new();

    [JsonPropertyName("file_id")] public string FileId { get; set; } = string.Empty;

    [JsonPropertyName("datetime")] public DateTime DateTime { get; set; }

    public class Handler : IServiceBusMessageHandler<CsvDataResponse>
    {
        private readonly AppDbContext _db;
        private readonly IMediator _mediator;
        private readonly ISignalRService _signalRService;

        public Handler(AppDbContext db, IMediator mediator, ISignalRService signalRService)
        {
            _db = db;
            _mediator = mediator;
            _signalRService = signalRService;
        }

        public async Task Handle(CsvDataResponse message, IDictionary<string, string> headers, DateTimeOffset timestamp,
            CancellationToken cancellationToken)
        {
            var fileData = message.FileId.Split("___");
            var fileName = fileData.FirstOrDefault();
            if (!Guid.TryParse(fileData.LastOrDefault(), out var fileId)) return;

            var engine = await _db.Set<Engine>()
                .FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken);

            if (engine == null)
            {
                var engineName = Path.GetFileNameWithoutExtension(fileName);
                engine = await _db.Set<Engine>()
                    .FirstOrDefaultAsync(x => x.Name == engineName && x.EngineType == EngineType.Live,
                        cancellationToken);
                if (engine == null)
                {
                    engine = new Engine
                    {
                        Id = fileId,
                        Name = engineName,
                        EngineType = EngineType.Live
                    };
                    _db.Set<Engine>().Add(engine);
                    await _db.SaveChangesAsync(cancellationToken);
                }
            }

            engine.EngineStatus = EngineStatus.Success;

            var entity = new Analysis
            {
                CageDefect = message.Defects.GetValueOrDefault("Дефект сепаратора", 0),
                OuterRingDefect = message.Defects.GetValueOrDefault("Дефект наружного кольца", 0),
                InnerRingDefect = message.Defects.GetValueOrDefault("Дефект внутреннего кольца", 0),
                RollingElementsDefect = message.Defects.GetValueOrDefault("Дефект тел качения", 0),
                Unbalance = message.Defects.GetValueOrDefault("Дисбаланс", 0),
                Misalignment = message.Defects.GetValueOrDefault("Расцентровка", 0),
                DateTime = message.DateTime,
            };

            engine.Analyses.Add(entity);

            await _db.SaveChangesAsync(cancellationToken);

            await NotifySignalr(engine.Id, cancellationToken);
        }

        private async Task NotifySignalr(Guid engineId, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetQuery { Id = engineId }, cancellationToken);
            await _signalRService.SendMessageToAllAsync(EngineHub.NewEngineNotificationMethodName, dto);            
        }
    }
}