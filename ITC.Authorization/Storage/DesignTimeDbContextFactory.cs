using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ITC.Authorization.Storage;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
{
    public ServiceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();
        var builder = new DbContextOptionsBuilder<ServiceDbContext>();

        var connectionString = configuration["DbConnectionString"];
        ((DbContextOptionsBuilder)builder).UseNpgsql(connectionString, o => o.MigrationsAssembly("ITC.Authorization"));
        return new ServiceDbContext(builder.Options);
    }
}