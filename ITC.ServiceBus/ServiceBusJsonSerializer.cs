using System.Text.Json;
using ITC.ServiceBus.Exceptions;
using ITC.ServiceBus.Interfaces;

namespace ITC.ServiceBus;

public class ServiceBusJsonSerializer<TMessage> : IServiceBusSerializer<TMessage>
{
    public string Serialize(TMessage payload)
    {
        return JsonSerializer.Serialize(payload);
    }

    public TMessage Deserealize(string serializedMessage)
    {
        var message = JsonSerializer.Deserialize<TMessage>(serializedMessage);
        if (message == null)
            throw new ConsumingDesiarelizationException<TMessage>("Value is null");

        return message;
    }
}