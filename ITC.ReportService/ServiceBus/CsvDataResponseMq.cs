using System.Text.Json.Serialization;
using ITC.Domain.Enums;
using ITC.Domain.Models;
using ITC.ReportService.Database;
using ITC.ServiceBus.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.ServiceBus;

public class CsvDataResponseMq
{
    public DefectsDto Defects { get; set; } = new DefectsDto();
    
    [JsonPropertyName("file_id")]
    public string FileId { get; set; } = string.Empty;
    
    public class DefectsDto
    {
        [JsonPropertyName("Дефект наружного кольца")]
        public double OuterRingDefect { get; set; }
        
        [JsonPropertyName("Дефект внутреннего кольца")]
        public double InnerRingDefect { get; set; }
        
        [JsonPropertyName("Дефект тел качения")]
        public double RollingElementsDefect { get; set; }
        
        [JsonPropertyName("Дефект сепаратора")]
        public double CageDefect { get; set; }
        
        [JsonPropertyName("Дисбаланс")]
        public double Unbalance { get; set; }
        
        [JsonPropertyName("Расцентровка")]
        public double Misalignment { get; set; }
    }
    
    public class Handler : IServiceBusMessageHandler<CsvDataResponseMq>
    {
        private readonly AppDbContext _db;

        public Handler (AppDbContext db)
        {
            _db = db;
        }
        public async Task Handle(CsvDataResponseMq message, IDictionary<string, string> headers, DateTimeOffset timestamp,
            CancellationToken cancellationToken)
        {
            var fileData = message.FileId.Split("___");
            var fileName = fileData.FirstOrDefault();
            if (!Guid.TryParse(fileData.LastOrDefault(), out var fileId)) return;
            
            var engine = await _db.Set<Engine>()
                .FirstOrDefaultAsync(x => x.Id == fileId, cancellationToken) ?? new Engine
            {
                Id = fileId,
                Name = fileName,
            };
            engine.EngineStatus = EngineStatus.Success;

            _db.Set<Engine>().Update(engine);
            
            var entity =  new Analysis
            {
                EngineId = fileId,
                CageDefect = message.Defects.CageDefect,
                OuterRingDefect = message.Defects.OuterRingDefect,
                InnerRingDefect = message.Defects.InnerRingDefect,
                RollingElementsDefect = message.Defects.RollingElementsDefect,
                Unbalance = message.Defects.Unbalance,
                Misalignment = message.Defects.Misalignment,
            };

            _db.Set<Analysis>().Update(entity);

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}