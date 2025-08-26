using System.Text.Json.Serialization;

namespace ITC.Domain.Dto;

public class DefectHistoryDto
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("probability")]
    public double Probability { get; set; }
}