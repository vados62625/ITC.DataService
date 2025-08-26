using System.Reflection;
using ITC.ReportService.Database;
using ITC.ReportService.Extensions.AspNetCore;
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

builder.Services.AddMemoryCache();
builder.Services.AddCors();

builder.Services.RegisterValidators();
builder.Services.AddProblemDetails(builder.Environment.IsDevelopment());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

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
app.UseCors(c => c.WithOrigins()
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
app.MapControllers();

app.Run();