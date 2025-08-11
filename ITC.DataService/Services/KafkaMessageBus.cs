using ITC.DataService.Interfaces;

namespace ITC.DataService.Services
{
    public class KafkaMessageBus<TKey, TValue> : IKafkaMessageBus<TKey, TValue>
    {
        private readonly KafkaProducer<TKey, TValue> _producer;
        public KafkaMessageBus(KafkaProducer<TKey, TValue> producer)
        {
            _producer = producer;
        }
        public async Task<TValue?> PublishAsync(TKey key, TValue message)
        {
            return await _producer.ProduceAsync(key, message) ? message : default;
        }
    }
}