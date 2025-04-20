using BugBucks.Shared.Messaging.Constants;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Topology;

/// <summary>
///     Async declarations of exchanges, queues and bindings for the Checkout saga.
///     Includes dead‐letter exchange/queue and message TTL.
/// </summary>
public static class RabbitMqTopology
{
    public static async Task DeclareCheckoutTopologyAsync(this IChannel channel)
    {
        // 1) Dead‑letter exchange & queue
        await channel.ExchangeDeclareAsync(
            RabbitMQConstants.CheckoutDeadLetterExchange,
            ExchangeType.Direct,
            RabbitMQConstants.Durable,
            RabbitMQConstants.AutoDelete);

        await channel.QueueDeclareAsync(
            RabbitMQConstants.CheckoutDeadLetterQueue,
            RabbitMQConstants.Durable,
            RabbitMQConstants.Exclusive,
            RabbitMQConstants.AutoDelete);

        await channel.QueueBindAsync(
            RabbitMQConstants.CheckoutDeadLetterQueue,
            RabbitMQConstants.CheckoutDeadLetterExchange,
            RabbitMQConstants.CheckoutRoutingKey);

        // 2) Main exchange
        await channel.ExchangeDeclareAsync(
            RabbitMQConstants.CheckoutExchange,
            ExchangeType.Direct,
            RabbitMQConstants.Durable,
            RabbitMQConstants.AutoDelete);

        // 3) Main queue with DLX + TTL
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", RabbitMQConstants.CheckoutDeadLetterExchange },
            { "x-dead-letter-routing-key", RabbitMQConstants.CheckoutRoutingKey },
            { "x-message-ttl", 60000 } // 60s TTL
        };

        await channel.QueueDeclareAsync(
            RabbitMQConstants.CheckoutQueue,
            RabbitMQConstants.Durable,
            RabbitMQConstants.Exclusive,
            RabbitMQConstants.AutoDelete,
            args);

        await channel.QueueBindAsync(
            RabbitMQConstants.CheckoutQueue,
            RabbitMQConstants.CheckoutExchange,
            RabbitMQConstants.CheckoutRoutingKey);
    }
}