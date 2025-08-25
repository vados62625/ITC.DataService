namespace ITC.ServiceBus.Exceptions;

public class ConsumingDesiarelizationException<TMessage> : Exception
{
    public ConsumingDesiarelizationException(string error) : base($"Target type {typeof(TMessage).Name}. Error : {error}")
    {
    }
}