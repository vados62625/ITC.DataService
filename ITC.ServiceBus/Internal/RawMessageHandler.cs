using System.Text;
using Confluent.Kafka;
using ITC.ServiceBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ITC.ServiceBus.Internal
{
    internal class RawMessageHandler<TMessage> : IServiceBusMessageHandler, IHostedService
    {
        private readonly ServiceBusConsumerHosted _hostedConsumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceBusSerializer<TMessage> _serializer;

        public RawMessageHandler(ServiceBusConsumerHosted hostedConsumer, IServiceScopeFactory scopeFactory, IServiceBusSerializer<TMessage> serializer)
        {
            _hostedConsumer = hostedConsumer;
            _scopeFactory = scopeFactory;
            _serializer = serializer;
        }

        public async Task HandleMessage(ConsumeResult<Null, string> cr, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var massageHandler = scope.ServiceProvider.GetRequiredService<IServiceBusMessageHandler<TMessage>>();

            var headersDictionary = cr.Message.Headers.ToDictionary(k => k.Key, h => Encoding.UTF8.GetString(h.GetValueBytes()));
            var timestamp = new DateTimeOffset(cr.Message.Timestamp.UtcDateTime);


            var messageValue = _serializer.Deserealize(cr.Message.Value);

            await massageHandler.Handle(messageValue, headersDictionary, timestamp, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _hostedConsumer.Subscribe<TMessage>();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
