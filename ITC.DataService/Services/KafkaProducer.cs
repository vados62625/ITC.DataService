using Confluent.Kafka;
using ITC.DataService.Config;
using Microsoft.Extensions.Options;

namespace ITC.DataService.Services;

public class KafkaProducer<TKey, TValue> : IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;

    public KafkaProducer(IOptions<KafkaProducerConfig> topicOptions, IProducer<TKey, TValue> producer)
    {
        _topic = topicOptions.Value.Topic;
        _producer = producer;
    }

    public async Task<bool> ProduceAsync(TKey key, TValue value)
    {
        try
        {
            var message = key is Null
                ? new Message<TKey, TValue> { Value = value }
                : new Message<TKey, TValue> { Key = key, Value = value };
            var result = await _producer.ProduceAsync(_topic, message);
            return result.Status == PersistenceStatus.Persisted;
        }
        catch (ProduceException<TKey, TValue> ex)
        {
            // Логируем ошибку
            Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
            return false;
        }
        catch (KafkaException ex)
        {
            Console.WriteLine($"Kafka error: {ex.Error.Reason}");
            return false;
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}