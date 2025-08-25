using Confluent.Kafka;

namespace ITC.ServiceBus.Interfaces;

public interface IServiceBusProducer
{
    public Task<DeliveryResult<Null, string>> Produce(
        string topic,
        string value,
        IDictionary<string, string>? headers = null,
        DateTimeOffset? timestamp = null,
        CancellationToken cancellationToken = default);
}