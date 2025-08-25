namespace ITC.ServiceBus.Interfaces;

internal interface IConsumersRegistry
{
    void RegisterConsumer<TMessage>();
    Type GetConsumerType(string consumerName);
    IDictionary<string, Type> RegisteredConsumers { get; }
}