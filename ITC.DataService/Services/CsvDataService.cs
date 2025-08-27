using System.Globalization;
using Confluent.Kafka;
using ITC.DataService.Dto;
using ITC.DataService.Interfaces;

namespace ITC.DataService.Services;

public class CsvDataService : ICsvDataService
{
    private readonly ILogger<CsvDataService> _logger;
    private readonly ICsvService _csvService;
    private readonly IKafkaMessageBus<Null, PhaseDataDto> _kafkaProducer;
    

    public CsvDataService(ILogger<CsvDataService> logger, ICsvService csvService, IKafkaMessageBus<Null, PhaseDataDto> kafkaProducer)
    {
        _logger = logger;
        _csvService = csvService;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<bool> UploadCsv(Stream fileStream, string fileName)
    {
        try
        {
            const int chunkSize = 25600 * 3; // 3 seconds

            var csvRows = _csvService.GetRowsData<IList<string>>(fileStream);

            // Обработка и отправка по чанкам
            var chunks = csvRows
                .Select(row => row.Select(x =>
                {
                    double.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out var phaseVal);
                    return phaseVal;
                }).ToArray())
                .Chunk(chunkSize);
            
            var fileId = $"{fileName}___{Guid.NewGuid().ToString()}";

            var dt = DateTime.Now;
            foreach (var chunk in chunks)
            {
                var dto = new PhaseDataDto
                {
                    Data = chunk,
                    FileId = fileId,
                    DateTime = dt,
                };
                await _kafkaProducer.PublishAsync(default!, dto);
                dt = dt.AddSeconds(1);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CSV file");
            return false;
        }
    }
}