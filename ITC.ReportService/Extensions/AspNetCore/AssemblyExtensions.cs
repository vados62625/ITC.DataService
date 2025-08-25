using System.Reflection;

namespace ITC.ReportService.Extensions.AspNetCore;

public static class AssemblyExtensions
{
    /// <summary>
    /// Возвращает все типы в сборке с указанным атрибутом
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypesWithAttribute<T>(this Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttributes(typeof(T), true).Length > 0)
            {
                yield return type;
            }
        }
    }
}