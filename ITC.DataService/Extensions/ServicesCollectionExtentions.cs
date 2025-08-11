using Confluent.Kafka;
using ITC.DataService.Config;
using ITC.DataService.Interfaces;
using ITC.DataService.Services;
using Microsoft.Extensions.Options;

namespace ITC.DataService.Extensions;

public static class ServicesCollectionExtentions
{
    public static IServiceCollection AddKafkaMessageBus(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton(typeof(IKafkaMessageBus<,>), typeof(KafkaMessageBus<,>));
    
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection services,
        Action<KafkaProducerConfig> configAction)
    {
        services.AddConfluentKafkaProducer<TKey, TValue>();

        services.AddSingleton<KafkaProducer<TKey, TValue>>();

        services.Configure(configAction);

        return services;
    }

    private static IServiceCollection AddConfluentKafkaProducer<TKey, TValue>(this IServiceCollection services)
    {
        services.AddSingleton(
            sp =>
            {
                var config = sp.GetRequiredService<IOptions<KafkaProducerConfig>>();

                var builder = new ProducerBuilder<TKey, TValue>(config.Value).SetValueSerializer(new KafkaSerializer<TValue>());

                return builder.Build();
            });

        return services;
    }
}