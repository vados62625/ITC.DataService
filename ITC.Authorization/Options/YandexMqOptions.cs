namespace ITC.Authorization.Options;

public class YandexMqOptions
{
    public required string TmAuthQueueUrl { get; set; } 
    public required string AccessKeyId { get; set; } 
    public required string SecretKey { get; set; } 
    public required string MessageGroup { get; set; }
}