namespace ITC.DataService.Interfaces;

public interface IKafkaProducer
{
    Task Produce(string payload);
}