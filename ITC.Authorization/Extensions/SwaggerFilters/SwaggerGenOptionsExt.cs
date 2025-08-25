using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ITC.Authorization.Extensions.SwaggerFilters;

public static class SwaggerGenOptionsExt
{
    /// <summary>
    /// Добавляет файл xml документации из указанного каталога.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="folderPath">Имя файла. Расширение xml добавляется если его нет.</param>
    /// <param name="fileName"></param>
    /// <param name="includeControllerXmlComments">Включить xml контроллеров</param>
    /// <returns></returns>
    public static SwaggerGenOptions IncludeXmlFile(this SwaggerGenOptions self, string folderPath, string fileName, bool includeControllerXmlComments = true)
    {
        if (!fileName.EndsWith(".xml"))
        {
            fileName += ".xml";
        }
        var xmlPath = Path.Combine(folderPath, fileName);
        self.IncludeXmlComments(xmlPath, includeControllerXmlComments);
        return self;
    }

    /// <summary>
    /// Добавляет файл xml документации из  AppContext.BaseDirectory. 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="fileName">Имя файла. Расширение xml добавляется если его нет.</param>
    /// <param name="includeControllerXmlComments">Включить xml контроллеров</param>
    /// <returns></returns>
    public static SwaggerGenOptions IncludeXmlFile(this SwaggerGenOptions self, string fileName, bool includeControllerXmlComments = true)
    {
        self.IncludeXmlFile(AppContext.BaseDirectory, fileName, includeControllerXmlComments);
        return self;
    }

    public static SwaggerGenOptions SwaggerDocVersion(this SwaggerGenOptions self, string apiVersion, string appVersion)
    {
        self.SwaggerDoc(apiVersion, new OpenApiInfo
        {
            Version = appVersion,
            Title = $"{Assembly.GetExecutingAssembly().GetName().Name}",
        });
        return self;
    }
    public static SwaggerGenOptions SwaggerDocVersion(this SwaggerGenOptions self, IEnumerable<string> apiVersions, string appVersion)
    {

        foreach (var apiVersion in apiVersions)
        {
            self.SwaggerDocVersion(apiVersion, appVersion);
        }
        return self;
    }
}