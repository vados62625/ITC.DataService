using System.Collections;

namespace ITC.Domain.Extensions;

public static class ObjectExtensions
{
    public static string ToQueryString(this object? obj, string uri)
    {
        return uri + obj.ToQueryString();
    }
    public static string ToQueryString(this object? obj)
    {
        var result = "";
        if (obj == null) 
            return result;
        
        var props = obj.GetType().GetProperties();
        foreach (var prop in props)
        {
            if (string.IsNullOrWhiteSpace(prop.Name)) continue;
            
            var value = prop.GetValue(obj, null);
            // значение свойства null, переходим к следующему
            if (value is null)
                continue;
            
            // IEnumerable, выводим как "?State=Created&State=PartiallySent"
            if (value is not string && value is IEnumerable arr)
            {
                foreach (var element in arr)
                {
                    result += $"&{prop.Name}={Uri.EscapeDataString(element?.ToString() ?? string.Empty)}";
                }
                continue;
            }

            //дату конвертнуть
            if (value is DateTimeOffset dtOff)
            {
                int hours = TimeZoneInfo.Local.BaseUtcOffset.Hours;
                string offset = string.Format("{0}{1}", ((hours > 0) ? "+" : ""), hours.ToString("00"));

                //var dtOffString = dtOff.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                var dtOffString = dtOff.ToString("yyyy-MM-ddTHH:mm:ss.fff") + offset;
                result += $"&{prop.Name}={Uri.EscapeDataString(dtOffString)}";
                continue;
            }

            // не массив не дата, выводим как обычно
            result += $"&{prop.Name}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}";
        }

        // поменять первый символ с & на ? 
        return string.IsNullOrEmpty(result) ? result : string.Concat("?", result.Substring(1));
    }
}