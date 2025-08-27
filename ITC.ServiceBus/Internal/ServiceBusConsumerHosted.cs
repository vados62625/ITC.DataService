using System.Diagnostics;
using System.Text;
using Confluent.Kafka;
using ITC.ServiceBus.Extensions;
using ITC.ServiceBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LoggerExtensions = ITC.ServiceBus.Extensions.LoggerExtensions;

namespace ITC.ServiceBus.Internal
{
    internal class ServiceBusConsumerHosted : IAsyncDisposable, IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumersRegistry _registry;
        private readonly ILogger<ServiceBusConsumerHosted> _log;

        private readonly IConsumer<Null, string> _consumer;

        private readonly CancellationTokenSource _cts = new();

        private readonly List<string> _topics = new();
        public ServiceBusConsumerHosted(
            ConsumerConfig config,
            IServiceProvider serviceProvider,
            IConsumersRegistry registry,
            ILogger<ServiceBusConsumerHosted> log)
        {
            _serviceProvider = serviceProvider;
            _registry = registry;
            _log = log;
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        public void Subscribe<TMessage>()
        {
            _registry.RegisterConsumer<TMessage>();
            var topic = $"{typeof(TMessage).Name}";
            _log.LogInformation("SUB {topic}", topic);
            _topics.Add(topic);
            _consumer.Subscribe(_topics);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000, _cts.Token);
                _log.LogInformation("Consuming started");
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumer.Consume(_cts.Token);
                        await TryConsume(cr, _cts.Token);
                    }
                    catch (Exception e)
                    {
                        _log.LogError(e.ToString());
                    }

                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private async Task TryConsume(ConsumeResult<Null, string> cr, CancellationToken cancellationToken)
        {
            try
            {
                using var activity = CreateActivity(cr);

                using var loggerScope = _log.BeginTraceIdScope(activity?.Id);

                _log.LogTrace("Try consume message from topic {topic}. Partition: {partition}, Offset: {offset}",
                    cr.TopicPartition.Topic, cr.TopicPartition.Partition.Value, cr.TopicPartitionOffset.Offset.Value);

                // var consumerName = cr.Topic.Replace(TopicConstants.TypedTopicPrefix, "");
                var consumerName = cr.Topic;

                var consumerType = _registry.GetConsumerType(consumerName);

                var rawConsumer = (IServiceBusMessageHandler)_serviceProvider.GetRequiredService(consumerType);

                if (string.IsNullOrEmpty(cr.Message.Value))
                {
                    _log.LogWarning("Message value is null");
                    return;
                }

                await rawConsumer.HandleMessage(cr, cancellationToken);

                activity?.Stop();
            }
            catch (Exception e)
            {
               _log.LogError(e.ToString());
            }
          
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Consuming stopping");
            _cts.Cancel();
            _consumer.Close();
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await CastAndDispose(_consumer);
            await CastAndDispose(_cts);

            return;

            static async ValueTask CastAndDispose(IDisposable resource)
            {
                if (resource is IAsyncDisposable resourceAsyncDisposable)
                    await resourceAsyncDisposable.DisposeAsync();
                else
                    resource.Dispose();
            }
        }
        private Activity? CreateActivity(ConsumeResult<Null, string> cr)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            var activity = new Activity(nameof(ServiceBusConsumerHosted));

            cr.Message.Headers.TryGetLastBytes(LoggerExtensions.TraceIdKey, out var bytes);

            if (bytes == null)
                return activity;

            var activityId = Encoding.UTF8.GetString(bytes);
            activity.SetParentId(activityId);
            activity.Start();
            return activity;
        }
    }
}
