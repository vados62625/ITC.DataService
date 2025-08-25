using System.Diagnostics;
using ITC.ServiceBus.Interfaces;
using ITC.ServiceBus.Internal;
using Microsoft.Extensions.Logging;
using LoggerExtensions = ITC.ServiceBus.Extensions.LoggerExtensions;

namespace ITC.ServiceBus;

public class ServiceBusMessagePublusher<TMessage> : IServiceBusMessagePublisher<TMessage>
{
    private readonly IServiceBusProducer _rawProducer;
    private readonly IServiceBusSerializer<TMessage> _serializer;
    private readonly ILogger<IServiceBusMessagePublisher<TMessage>> _log;

    public ServiceBusMessagePublusher(IServiceBusProducer rawProducer, IServiceBusSerializer<TMessage> serializer, ILogger<IServiceBusMessagePublisher<TMessage>> log)
    {
        _rawProducer = rawProducer;
        _serializer = serializer;
        _log = log;
    }

    public async Task Produce(TMessage message,
        IDictionary<string, string>? headers = null,
        DateTimeOffset? timestamp = null,
        CancellationToken cancellationToken = default)
    {
        var json = _serializer.Serialize(message);
        var topic = $"{TopicConstants.TypedTopicPrefix}{typeof(TMessage).Name}";

        headers ??= new Dictionary<string, string>();

        if(!headers.ContainsKey(LoggerExtensions.TraceIdKey))
            headers.Add(new KeyValuePair<string, string>(LoggerExtensions.TraceIdKey, Activity.Current?.Id ?? Guid.NewGuid().ToString()));

        _log.LogInformation("Send message type {type} to topic {topic}", 
            typeof(TMessage).Name, topic);

        await _rawProducer.Produce(topic, json, headers, timestamp, cancellationToken);
    }

}