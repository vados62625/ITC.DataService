using Confluent.Kafka;
using ITC.DataService.Dto;
using ITC.DataService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ITC.DataService.Controllers;

[ApiController]
[Route("[controller]")]
public class CsvController : ControllerBase
{
    private readonly ILogger<CsvController> _logger;
    private readonly ICsvDataService _csvDataService;

    public CsvController(ILogger<CsvController> logger, ICsvDataService csvDataService)
    {
        _logger = logger;
        _csvDataService = csvDataService;
    }

    [HttpPost("Upload")]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
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
            var success = await _csvDataService.UploadCsv(stream);
            if (!success)
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