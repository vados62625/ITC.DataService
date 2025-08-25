namespace ITC.Domain.Extensions;

public static class StringExtensions
{
    public static string AppendTraceId(this string message, string? traceId)
    {
        return traceId == null ? message : string.Concat(message, " ", traceId);
    }
    public static string GetStringFromIntArray(this int[]? array)
    {
        var str = "";

        if (array == null)
            return str;

        if (array.Length == 0)
            return str;

        foreach (var number in array)
        {
            str += $"{number}, ";
        }
        return str[..^2];
    }

    public static string Base64ToImageSource(this string self)
    {
        return $"data:image/jpg;base64,{self}";
    }
    
    public static string GetExtensionFromContentType(this string contentType)
    {
        return contentType.ToLower() switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/gif" => "gif",
            "image/webp" => "webp",
            "image/svg+xml" => "svg",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}