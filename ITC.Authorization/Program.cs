using NLog;
using NLog.Web;
using System.Reflection;
using System.Security.Claims;
using Keycloak.Net;
using Microsoft.Extensions.Options;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authorization;
using ITC.Authorization.Background;
using ITC.Authorization.Extensions.AspNetCore;
using ITC.Authorization.Middleware;
using ITC.Authorization.Options;
using ITC.Authorization.Services;
using ITC.Authorization.Storage;
using ITC.CQRS.Extensions;
using ITC.ServiceBus.Exceptions;
using ITC.Storage.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;

var builder = WebApplication.CreateBuilder(args);

var logger = LogManager
    .Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

var version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

logger.Info($"Init main. App ver. {version}");

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
var controllersVersions = assemblies.GetApiControllerVersions().Select(c => $"v{c}").ToArray();

builder.Services.AddServiceDbContext<ServiceDbContext>(builder.Configuration["DbConnectionString"]!);

builder.Services.Configure<KeycloakClientOptions>(builder.Configuration.GetSection("Keycloak"));

// builder.Services.AddHostedService<UpdateUserRolesBackgroundService>();
builder.Services.AddScoped<IUserRolesCacheService, UserRolesCacheService>();
builder.Services.AddSingleton<ISystemClock>(new SystemClock());

IdentityModelEventSource.ShowPII = true;
builder.Services.AddScoped(c =>
{
    var clientOptions = c.GetRequiredService<IOptions<KeycloakClientOptions>>();
    return new KeycloakClient(
        clientOptions.Value.Url,
        clientOptions.Value.ClientSecret,
        new KeycloakOptions(clientOptions.Value.ClientName));
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, ProtectedResourcePolicyProvider>();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddKeycloakAuthorization(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("users", policy => policy.RequireAssertion(context => context.User.HasClaim(c => c.Value == "user" || c.Value == "admin")));
    options.AddPolicy("[admin]", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "admin"));
    options.AddPolicy("noaccess", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "noaccess"));
    options.AddPolicy("UsersRead", b => b.RequireProtectedResource("users", "view"));
});

var serviceBusConfigSection = builder.Configuration.GetSection("ServiceBusOptions");

builder.Services
    .AddFGCqrs<ServiceDbContext>()
    .AddFGServiceBusConsumer(serviceBusConfigSection)
    .AddFGServiceBusProducer(serviceBusConfigSection)
    .AddMessageBusServices()
    .AddEntityUpdateEventServices();

builder.Services.AddSwagger(controllersVersions);
builder.Services.AddVersioning();

builder.Host.UseNLog();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
await using var db = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
await db.Database.MigrateAsync();

app.UseCors(c => c.WithOrigins()
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseVersionedSwaggerUI(controllersVersions);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.Run();
