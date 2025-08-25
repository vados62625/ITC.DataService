using System.Text;
using Confluent.Kafka;
using ITC.ServiceBus.Interfaces;
using Microsoft.Extensions.Logging;

namespace ITC.ServiceBus.Internal
{
    internal class ServiceBusRawProducer : IAsyncDisposable, IServiceBusProducer
    {
        private readonly ILogger<ServiceBusRawProducer> _log;
        private readonly IProducer<Null, string> _producer;

        public ServiceBusRawProducer(ProducerConfig config, ILogger<ServiceBusRawProducer> log)
        {
            _log = log;
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task<DeliveryResult<Null, string>> Produce(
            string topic,
            string value,
            IDictionary<string, string>? headers = null,
            DateTimeOffset? timestamp = null,
            CancellationToken cancellationToken = default)
        {

            var message = new Message<Null, string>
            {
                Value = value
            };

            if (headers != null)
            {
                message.Headers = new();

                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
                }
            }

            if (timestamp != null)
                message.Timestamp = new Timestamp(timestamp.Value);


            var result = await _producer.ProduceAsync(topic, message, cancellationToken);

            _log.LogTrace("Produced to topic {topic}. Partition: {partition}, Offset: {offset}",
                result.TopicPartition.Topic, result.TopicPartition.Partition.Value, result.TopicPartitionOffset.Offset.Value);

            return result;
        }

        public ValueTask DisposeAsync()
        {
            _producer.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
