using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ITC.DataService.Dto;

public class PhaseDataDto
{
    [JsonPropertyName("data")]
    [JsonProperty("data")]
    public double[][] Data { get; set; }
    public string FileId { get; set; }
}