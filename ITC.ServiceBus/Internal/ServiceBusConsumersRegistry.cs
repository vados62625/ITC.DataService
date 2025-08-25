using ITC.ServiceBus.Interfaces;
using Microsoft.Extensions.Logging;

namespace ITC.ServiceBus.Internal;

internal class ServiceBusConsumersRegistry : IConsumersRegistry
{
    private readonly ILogger<ServiceBusConsumerHosted> _log;
    public IDictionary<string, Type> RegisteredConsumers { get; } = new Dictionary<string, Type>();

    public ServiceBusConsumersRegistry(ILogger<ServiceBusConsumerHosted> log)
    {
        _log = log;
    }

    public void RegisterConsumer<TMessage>()
    {
        var consumerType = typeof(RawMessageHandler<TMessage>);
        var consumerName = typeof(TMessage).Name;
        _log.LogTrace("Add {consumerName} {consumerType}", consumerName, consumerType);
        RegisteredConsumers.Add(consumerName, consumerType);
    }

    public Type GetConsumerType(string consumerName)
    {
        return RegisteredConsumers[consumerName];
    }
}