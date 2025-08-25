using System.Text.Json.Serialization;
using ITC.Domain.Extensions;

namespace ITC.Domain.Dto;

[JsonConverter(typeof(ProblemDetailsJsonConverter))]
public class ProblemDetailsDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
        
    [JsonPropertyName("title")]
    public string? Title { get; set; }
        
    [JsonPropertyName("status")]
    public int? Status { get; set; }
        
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
        
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }
        
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
        
    [JsonExtensionData]
    public IDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);
}