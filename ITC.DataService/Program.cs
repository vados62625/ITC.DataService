using Confluent.Kafka;
using ITC.DataService.Config;
using ITC.DataService.Dto;
using ITC.DataService.Extensions;
using ITC.DataService.Interfaces;
using ITC.DataService.Services;
using ITC.DataService.Services.Hosted;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<DataObserverConfig>(configuration.GetSection("DataObserverConfiguration"));

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaMessageBus();
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<ICsvDataService, CsvDataService>();

builder.Services.AddHostedService<DataObserverHostedService>();

builder.Services.AddKafkaProducer<Null, PhaseDataDto>(p =>
{
    var kafkaSection = configuration.GetSection("KafkaConfiguration");
    
    foreach (var property in p.GetType().GetProperties())
    {
        var configValue = kafkaSection.GetValue<string>(property.Name);
        if (string.IsNullOrEmpty(configValue)) continue;
        
        var convertedValue = GetTypedValue(property.PropertyType, configValue);
        property.SetValue(p, convertedValue);
    }
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 300 * 1024 * 1024; // 300 MB
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.ConfigObject = new ConfigObject
        {
            ShowCommonExtensions = true
        };
    });
app.UseCors(c => c.WithOrigins()
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
app.MapControllers();
app.Run();

object? GetTypedValue(Type propertyType, string configValue)
{
    var underlyingType = Nullable.GetUnderlyingType(propertyType);
    if (underlyingType != null)
        propertyType = underlyingType;
    
    if (propertyType.IsEnum)
    {
        return Enum.Parse(propertyType, configValue);
    }
    return Convert.ChangeType(configValue, propertyType);
}