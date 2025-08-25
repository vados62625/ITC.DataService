namespace ITC.ServiceBus.Interfaces;

public interface IServiceBusMessagePublisher<in TMessage>
{
    Task Produce(TMessage message,
        IDictionary<string, string>? headers = null,
        DateTimeOffset? timestamp = null,
        CancellationToken cancellationToken = default);
}