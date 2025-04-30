using System;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using BugBucks.Shared.Messaging.Abstractions.Messaging;

namespace BugBucks.Shared.Messaging.Infrastructure.RabbitMq
{
    internal class RabbitMqConsumer : IMessageConsumer
    {
        private readonly IRabbitMqConnectionFactory _factory;

        public RabbitMqConsumer(IRabbitMqConnectionFactory factory) => _factory = factory;

        public async Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class
        {
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await RabbitMqTopology.DeclareAsync(channel);

            var queueName = RabbitMqTopology.QueuePrefix + typeof(TEvent).Name;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var evt = JsonSerializer.Deserialize<TEvent>(body);
                if (evt != null)
                {
                    await handler(evt);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }
    }
}
