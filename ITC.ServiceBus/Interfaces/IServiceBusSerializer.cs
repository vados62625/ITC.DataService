namespace ITC.ServiceBus.Interfaces;

public interface IServiceBusSerializer<TMessage>
{
    string Serialize(TMessage payload);
    TMessage Deserealize(string serializedMessage);
}