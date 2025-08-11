namespace ITC.DataService.Interfaces
{
    public interface IKafkaMessageBus<in TKey, TValue>
    {
        Task<TValue?> PublishAsync(TKey key, TValue message);
    }
}