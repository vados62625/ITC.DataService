using System.Reflection;
using Asp.Versioning;

namespace ITC.CQRS.Extensions;

public static class AssemblyExtensions
{
    /// <summary>
    /// Возвращает все версии контроллеров в сборке
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IEnumerable<string> GetApiControllerVersions(this Assembly[] assemblies)
    {
        var types = new List<Type>();
        foreach (var assembly in assemblies)
        {
            types.AddRange(assembly.GetTypesWithAttribute<ApiVersionAttribute>());
        }

        var versions = new List<string>();
        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes<ApiVersionAttribute>();
            foreach (var attribute in attributes)
            {
                versions.AddRange(attribute.Versions.Select(c => c.ToString()));
            }
        }

        return versions.Distinct();
    }
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