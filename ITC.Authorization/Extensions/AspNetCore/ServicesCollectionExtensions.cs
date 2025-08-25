using Asp.Versioning;
using ITC.Authorization.CQRS;
using ITC.Authorization.Extensions.SwaggerFilters;
using ITC.Authorization.ServiceBus;
using ITC.Authorization.ServiceBus.Notifications;
using ITC.Authorization.ServiceBus.Organization;
using ITC.ServiceBus;
using ITC.ServiceBus.Exceptions;
using Microsoft.OpenApi.Models;

namespace ITC.Authorization.Extensions.AspNetCore;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddVersioning(this IServiceCollection self)
    {
        self.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return self;
    }
    public static IServiceCollection AddSwagger(this IServiceCollection self, string[] apiControllerVersions)
    {
        var appVersion = typeof(Program).Assembly.GetName()!.Version!.ToString();

        self.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(t => t.ToString());
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] " +
                    "and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });

            options

                .SwaggerDocVersion(apiControllerVersions, appVersion)
                .IncludeXmlFile("SwaggerDoc");

            options.OperationFilter<RequestIdHeaderFilter>();
            options.OperationFilter<ReqiuredAuthPolicyDescriptionFilter>();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var basePath = env != "Development" ? "/authorization" : "/";
            options.DocumentFilter<BasePathDocumentFilter>(basePath);
        });

        return self;
    }

    public static IServiceCollection AddMessageBusServices(this IServiceCollection self)
    {
        self
            .AddServiceBusSerializer<UserSetBrokerIdCommand, ServiceBusJsonSerializer<UserSetBrokerIdCommand>>()
            .AddServiceBusSerializer<UserSetCompanyIdCommand, ServiceBusJsonSerializer<UserSetCompanyIdCommand>>()
            .AddServiceBusSerializer<UserSetEmployeeIdCommand, ServiceBusJsonSerializer<UserSetEmployeeIdCommand>>()
            .AddServiceBusSerializer<InitialCreateBrokerAndCompanyMq, ServiceBusJsonSerializer<InitialCreateBrokerAndCompanyMq>>()
            .AddServiceBusSerializer<InitialCreateBrokerAndCompanyResultMq, ServiceBusJsonSerializer<InitialCreateBrokerAndCompanyResultMq>>()
            .AddServiceBusSerializer<SendEmailWithVerificationCodeMq, ServiceBusJsonSerializer<SendEmailWithVerificationCodeMq>>()
            .AddServiceBusSerializer<SendEmailWithInviteTokenMq, ServiceBusJsonSerializer<SendEmailWithInviteTokenMq>>()
            .AddServiceBusSerializer<UpdateUserRolesMq, ServiceBusJsonSerializer<UpdateUserRolesMq>>()
            .AddServiceBusSerializer<UserEmailConfirmedMq, ServiceBusJsonSerializer<UserEmailConfirmedMq>>()
            .AddServiceBusSerializer<SendPlatformNotificationCommand, ServiceBusJsonSerializer<SendPlatformNotificationCommand>>()
            .AddServiceBusSerializer<SendEmailWithPasswordResetLinkMq, ServiceBusJsonSerializer<SendEmailWithPasswordResetLinkMq>>()
            
            .AddPublisher<InitialCreateBrokerAndCompanyMq>()
            .AddPublisher<SendEmailWithVerificationCodeMq>()
            .AddPublisher<SendEmailWithInviteTokenMq>()
            .AddPublisher<UpdateUserRolesMq>()
            .AddPublisher<UserEmailConfirmedMq>()
            .AddPublisher<SendEmailWithPasswordResetLinkMq>()

            .AddMessageHandler<InitialCreateBrokerAndCompanyResultMq, InitialCreateBrokerAndCompanyResultMq.Handler>()
            .AddMessageHandler<UserSetCompanyIdCommand, UserSetCompanyIdCommandHandler>()
            .AddMessageHandler<UserSetBrokerIdCommand, UserSetBrokerIdCommandHandler>()
            .AddMessageHandler<UserSetEmployeeIdCommand, UserSetEmployeeIdCommandHandler>()
            .AddPublisher<SendPlatformNotificationCommand>();

        return self;
    }

    public static IServiceCollection AddEntityUpdateEventServices(this IServiceCollection self)
    {
        self.AddServiceBusSerializer<EntityUpdateEvent, ServiceBusJsonSerializer<EntityUpdateEvent>>()
            .AddPublisher<EntityUpdateEvent>();
        self.AddServiceBusSerializer<EntityUpdateEventResult, ServiceBusJsonSerializer<EntityUpdateEventResult>>()
            .AddMessageHandler<EntityUpdateEventResult, EntityUpdateEventResult.Handler>();

        return self;
    }
}