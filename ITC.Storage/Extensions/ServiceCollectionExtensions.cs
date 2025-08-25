using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace ITC.Storage.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDbContext<TDbContext>(this IServiceCollection services, string connectionString)
        where TDbContext : DbContext
    {
        var clock = new SystemClock();
        services.AddDbContext<TDbContext>(c =>
        {
            c.UseNpgsql(connectionString);
            c.AddInterceptors(new EntityBaseInterceptor(clock));
        });
        return services;
    }
}