using Confluent.Kafka;

namespace ITC.DataService.Config;

public class KafkaProducerConfig : ProducerConfig
{
    public required string Topic { get; set; }
}