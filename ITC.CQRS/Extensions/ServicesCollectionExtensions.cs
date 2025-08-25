using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ITC.CQRS.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddFGCqrs<TAssembly>(this IServiceCollection self)
    {
        return self.RegisterValidators<TAssembly>()
            .AddAutoMapper(typeof(TAssembly))
            .AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
    }
    public static IServiceCollection RegisterValidators<TAssembly>(this IServiceCollection self)
    {
        var validators = AssemblyScanner.FindValidatorsInAssemblyContaining<TAssembly>();
        validators.ForEach(validator => self.AddScoped(validator.InterfaceType, validator.ValidatorType));
        return self;
    }
}