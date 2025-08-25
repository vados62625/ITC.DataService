namespace ITC.Authorization.Extensions.AspNetCore;

public static class MicrosoftLoggerExtentions
{

    public static void Debug(this ILogger log, string message)
    {
        log.LogDebug(message);
    }
    public static void Trace(this ILogger log, string message)
    {
        log.LogTrace(message);
    }

    public static void Info(this ILogger log, string message)
    {
        log.LogInformation(message);
    }

    public static void Warn(this ILogger log, string message)
    {
        log.LogWarning(message);
    }

    public static void Error(this ILogger log, string message)
    {
        log.LogError(message);
    }

    public static void Error(this ILogger log, Exception ex)
    {
        log.LogError(ex.ToString());
    }

    public static void Fatal(this ILogger log, string message)
    {
        log.LogCritical(message);
    }

    public static void Fatal(this ILogger log, Exception ex)
    {
        log.LogCritical(ex.ToString());
    }
}