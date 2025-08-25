using Confluent.Kafka;

namespace ITC.ServiceBus.Interfaces;

public interface IServiceBusMessageHandler
{
    Task HandleMessage(ConsumeResult<Null, string> cr, CancellationToken cancellationToken);
}
public interface IServiceBusMessageHandler<in TMessage>
{
    public Task Handle(TMessage message, IDictionary<string, string> headers, DateTimeOffset timestamp, CancellationToken cancellationToken);
}