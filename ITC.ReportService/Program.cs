using System.Reflection;
using ITC.CQRS.Extensions;
using ITC.ReportService.Database;
using ITC.ReportService.Extensions.AspNetCore;
using ITC.ReportService.Hub;
using ITC.ReportService.Services;
using ITC.ServiceBus.Exceptions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using SystemClock = Microsoft.Extensions.Internal.SystemClock;

var clock = new SystemClock();

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddSignalR();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddHttpContextAccessor();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresqlContext"));
    options.AddInterceptors(new EntityBaseInterceptor(clock));
});
builder.Services.AddScoped<DbContext>(provider => provider.GetService<AppDbContext>()!);

builder.Services.AddScoped<ISignalRService, SignalRService>();

builder.Services.AddMemoryCache();
builder.Services.AddCors();

var serviceBusConfigSection = builder.Configuration.GetSection("ServiceBusOptions");

builder.Services
    .AddFGCqrs<AppDbContext>()
    .AddFGServiceBusConsumer(serviceBusConfigSection)
    .AddFGServiceBusProducer(serviceBusConfigSection)
    .AddMessageBusServices();

builder.Services.RegisterValidators();
builder.Services.AddProblemDetails(builder.Environment.IsDevelopment());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddHttpClient<IDataServiceClient, DataServiceClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["DataServiceBaseAddress"]!);
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 300 * 1024 * 1024; // 300 MB
});
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
});

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.ConfigObject = new ConfigObject
        {
            ShowCommonExtensions = true,
        };
    });
app.UseCors(c => c.WithOrigins("http://89.108.73.166", "http://7-volt.ru")
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed((x) => true)
    .AllowCredentials());

app.MapControllers();
app.MapHub<EngineHub>("/engineHub");

app.Run();