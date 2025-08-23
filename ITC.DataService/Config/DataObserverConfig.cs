using Confluent.Kafka;

namespace ITC.DataService.Config;

public class DataObserverConfig : ProducerConfig
{
    public required string CsvDirPath { get; set; }
    public int Interval { get; set; } = 60; // интервал в секундах
}