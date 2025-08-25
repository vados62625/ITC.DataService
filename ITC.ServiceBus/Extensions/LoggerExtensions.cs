using Microsoft.Extensions.Logging;

namespace ITC.ServiceBus.Extensions;

public static class LoggerExtensions
{
    public const string TraceIdKey = "trace-id";
    public static IDisposable? BeginTraceIdScope(this ILogger logger, string? traceId)
    {
        var kv = new Dictionary<string, object>
        {
            { TraceIdKey, traceId ?? "" }
        };
        return logger.BeginScope(kv);
    }
}