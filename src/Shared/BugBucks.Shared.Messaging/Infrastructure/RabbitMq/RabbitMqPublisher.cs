using System;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Contracts.Events;

namespace BugBucks.Shared.Messaging.Infrastructure.RabbitMq
{
    internal class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnectionFactory _factory;

        public RabbitMqPublisher(IRabbitMqConnectionFactory factory) => _factory = factory;

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await RabbitMqTopology.DeclareAsync(channel);

            var body = JsonSerializer.SerializeToUtf8Bytes(@event);
            var props = new BasicProperties
            {
                Persistent = true
            };

            var routingKey = @event switch
            {
                OrderCreatedEvent _ => "order.created",
                PaymentRequestedEvent _ => "payment.requested",
                PaymentSucceededEvent _ => "payment.succeeded",
                PaymentFailedEvent _ => "payment.failed",
                InventoryReserveRequestedEvent _ => "inventory.reserve.requested",
                InventoryReservedEvent _ => "inventory.reserved",
                InventoryFailedEvent _ => "inventory.failed",
                OrderCompletedEvent _ => "order.completed",
                OrderCompensatedEvent _ => "order.compensated",
                _ => throw new InvalidOperationException($"Unsupported event type {@event.GetType().Name}")
            };

            await channel.BasicPublishAsync(exchange: RabbitMqTopology.ExchangeName,
                                            routingKey: routingKey,
                                            mandatory: false,
                                            basicProperties: props,
                                            body: body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
