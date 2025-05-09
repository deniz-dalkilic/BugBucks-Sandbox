using BugBucks.Shared.Messaging.Contracts.Events;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Infrastructure.RabbitMq;

public static class RabbitMqTopology
{
    public const string ExchangeName = "BugBucks.Exchange";
    public const string QueuePrefix = "BugBucks.Queue.";

    public static async Task DeclareAsync(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, true);

        await DeclareQueueAsync(channel, QueuePrefix + nameof(OrderCreatedEvent), "order.created");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(PaymentRequestedEvent), "payment.requested");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(PaymentSucceededEvent), "payment.succeeded");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(PaymentFailedEvent), "payment.failed");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(InventoryReserveRequestedEvent),
            "inventory.reserve.requested");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(InventoryReservedEvent), "inventory.reserved");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(InventoryFailedEvent), "inventory.failed");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(OrderCompletedEvent), "order.completed");
        await DeclareQueueAsync(channel, QueuePrefix + nameof(OrderCompensatedEvent), "order.compensated");
    }

    public static async Task DeclareQueueAsync(IChannel channel, string queueName, string routingKey)
    {
        var dlxExchange = queueName + ".dlx";
        var dlqQueue = queueName + ".dlq";

        await channel.ExchangeDeclareAsync(dlxExchange, ExchangeType.Fanout, true);
        await channel.QueueDeclareAsync(dlqQueue, true, false, false);
        await channel.QueueBindAsync(dlqQueue, dlxExchange, "");

        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", dlxExchange }
        };

        await channel.QueueDeclareAsync(queueName, true, false, false, args);
        await channel.QueueBindAsync(queueName, ExchangeName, routingKey);
    }
}