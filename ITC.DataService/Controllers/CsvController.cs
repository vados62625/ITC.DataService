using Confluent.Kafka;
using ITC.DataService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ITC.DataService.Controllers;

[ApiController]
[Route("[controller]")]
public class CsvController : ControllerBase
{
    private readonly ILogger<CsvController> _logger;
    private readonly IKafkaMessageBus<Null, IDictionary<int, IList<string>>> _kafkaProducer;
    private readonly ICsvService _csvService;

    public CsvController(ILogger<CsvController> logger, IKafkaMessageBus<Null, IDictionary<int, IList<string>>> kafkaProducer, ICsvService csvService)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _csvService = csvService;
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> UploadCsv(IFormFile? file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (file.ContentType != "text/csv" && Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                return BadRequest("Only CSV files are allowed");
            }

            await using var stream = file.OpenReadStream();

            // Обработка CSV
            var data = _csvService.GetRowsData<IList<string>>(stream)
                .Select((value, index) => new { value, index })
                .ToDictionary(pair => pair.index, pair => pair.value);

            // Отправка в Kafka
            var sendResult = await _kafkaProducer.PublishAsync(default!, (Dictionary<int, IList<string>>)data);

            if (sendResult == null)
            {
                return StatusCode(500, "Failed to send data to Kafka");
            }

            return Ok("CSV processed and sent to Kafka successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CSV file");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}