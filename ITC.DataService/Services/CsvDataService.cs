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

    public async Task<bool> UploadCsv(Stream fileStream)
    {
        try
        {
            // Обработка CSV
            var data = _csvService.GetRowsData<IList<string>>(fileStream)
                // .Select((value, index) => new { value, index })
                .Select(c => c.Select(x => float.TryParse(x, out var phaseVal) ? phaseVal : 0).ToArray())
                .ToArray();

            var dto = new PhaseDataDto()
            {
                Data = data
            };

            // Отправка в Kafka
            var sendResult = await _kafkaProducer.PublishAsync(default!, dto);

            if (sendResult == null)
            {
                return false;
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