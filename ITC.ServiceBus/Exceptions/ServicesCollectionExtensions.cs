using Confluent.Kafka;
using ITC.ServiceBus.Interfaces;
using ITC.ServiceBus.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITC.ServiceBus.Exceptions;

public static class ServicesCollectionExtensions
{
    private const string SslCaLocation = "YandexCA.pem";
    private static readonly string SslCaLocationPath = Path.Combine(AppContext.BaseDirectory, SslCaLocation);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration">Секция appsettings</param>
    /// <param name="configAction">https://docs.confluent.io/platform/7.5/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html</param>
    /// <returns></returns>
    public static IServiceCollection AddFGServiceBusConsumer(this IServiceCollection serviceCollection, IConfiguration configuration,
        Action<ConsumerConfig>? configAction = null)
    {
        var config = new ConsumerConfig
        {
            SslCaLocation = SslCaLocationPath,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.ScramSha512
        };
        configuration.Bind(config);
        configAction?.Invoke(config);
        serviceCollection.AddSingleton<IConsumersRegistry, ServiceBusConsumersRegistry>();
        serviceCollection.AddSingleton(c => new ServiceBusConsumerHosted(
            config, c,
            c.GetRequiredService<IConsumersRegistry>(), 
            c.GetRequiredService<ILogger<ServiceBusConsumerHosted>>()));

        serviceCollection.AddHostedService(c => c.GetRequiredService<ServiceBusConsumerHosted>());
        return serviceCollection;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configuration">Секция appsettings</param>
    /// <param name="configAction">https://docs.confluent.io/platform/7.5/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html</param>
    /// <returns></returns>
    public static IServiceCollection AddFGServiceBusProducer(this IServiceCollection serviceCollection, IConfiguration configuration,
        Action<ProducerConfig>? configAction = null)
    {
        var config = new ProducerConfig
        {
            SslCaLocation = SslCaLocationPath,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.ScramSha512
        };

        configuration.Bind(config);
        configAction?.Invoke(config);
        serviceCollection.AddSingleton<IServiceBusProducer>(c =>
            new ServiceBusRawProducer(config, c.GetRequiredService<ILogger<ServiceBusRawProducer>>()));
        return serviceCollection;
    }
  
    /// <summary>
    /// Добавляет обработчик сообщений
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <typeparam name="THandler">Тип обработчика</typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddMessageHandler<TMessage, THandler>(this IServiceCollection serviceCollection)
        where THandler : class, IServiceBusMessageHandler<TMessage>
    {
        serviceCollection.AddSingleton<RawMessageHandler<TMessage>>();
        serviceCollection.AddHostedService(c => c.GetRequiredService<RawMessageHandler<TMessage>>());
        serviceCollection.AddScoped<IServiceBusMessageHandler<TMessage>, THandler>();
        return serviceCollection;
    }
    /// <summary>
    /// Добавляет сериализатор для Producer и Consumer
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TSerializer"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddServiceBusSerializer<TMessage, TSerializer>(this IServiceCollection serviceCollection)
        where TSerializer : class, IServiceBusSerializer<TMessage>
    {
        serviceCollection.AddSingleton<IServiceBusSerializer<TMessage>, TSerializer>();
        return serviceCollection;
    }

    /// <summary>
    /// Добавляет публикатора сообщений
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения</typeparam>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    public static IServiceCollection AddPublisher<TMessage>(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IServiceBusMessagePublisher<TMessage>, ServiceBusMessagePublusher<TMessage>>();
        return serviceCollection;
    }
}