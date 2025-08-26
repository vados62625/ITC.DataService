using System.Globalization;
using System.Reflection;
using FluentValidation;
using FluentValidation.TestHelper;
using Hellang.Middleware.ProblemDetails;
using ITC.Domain.Dto;
using ITC.ReportService.ServiceBus;
using ITC.ServiceBus;
using ITC.ServiceBus.Exceptions;
using ProblemDetailsFactory = Hellang.Middleware.ProblemDetails.ProblemDetailsFactory;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;  

namespace ITC.ReportService.Extensions.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProblemDetails(this IServiceCollection self, bool isDevelopment)
    {

        self.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) => isDevelopment;
            options.OnBeforeWriteDetails = (ctx, problem) =>
            {
                problem.Extensions["traceId"] = ctx.TraceIdentifier;
                problem.Extensions["requestId"] = ctx.Request.Headers["Request-Id"].ToString();
            };

            //TODO маппинг исключений в http коды
            options.ValidationProblemStatusCode = 400;
            options.MapFluentValidationException();
            options.MapToStatusCode<UnauthorizedException>(StatusCodes.Status401Unauthorized);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

        });
        return self;
    }
    public static void MapFluentValidationException(this ProblemDetailsOptions options)
    {
        options.Map<ValidationTestException>((ctx, ex) =>
        {
            var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(c => c.ErrorMessage).ToArray());

            return factory.CreateValidationProblemDetails(ctx, errors);
        });
    }
    
    public static IServiceCollection RegisterValidators(this IServiceCollection self)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(Assembly.Load).Append(Assembly.GetExecutingAssembly());
        var validators = assemblies
            .SelectMany(c => AssemblyScanner.FindValidatorsInAssembly(c))
            .ToList();
        validators.ForEach(validator => self.AddScoped(validator.InterfaceType, validator.ValidatorType));
        return self;
    }
    
    public static IServiceCollection AddMessageBusServices(this IServiceCollection self)
    {
        self
            .AddServiceBusSerializer<CsvDataResponse, ServiceBusJsonSerializer<CsvDataResponse>>()

            .AddMessageHandler<CsvDataResponse, AnalysisResultHandler>();

        return self;
    }
}