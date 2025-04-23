using System.Text.Json;
using BugBucks.Shared.Messaging.Constants;
using BugBucks.Shared.Messaging.Events;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Services;

/// <summary>
///     Publishes saga-related events to RabbitMQ asynchronously using a lazily initialized channel.
/// </summary>
public class CheckoutSagaPublisher : IAsyncDisposable
{
    private readonly Lazy<Task<IChannel>> _channelTask;
    private readonly IConnection _connection;

    public CheckoutSagaPublisher(IConnection connection)
    {
        _connection = connection;
        _channelTask = new Lazy<Task<IChannel>>(
            () => _connection.CreateChannelAsync());
    }

    public async ValueTask DisposeAsync()
    {
        if (_channelTask.IsValueCreated)
        {
            var channel = await _channelTask.Value.ConfigureAwait(false);
            await channel.CloseAsync();
            channel.Dispose();
        }
    }

    public ValueTask PublishPaymentRequestedAsync(Guid orderId, string paymentDetails, string idempotencyKey)
    {
        return PublishAsync(new PaymentRequestedEvent(orderId, paymentDetails, idempotencyKey));
    }

    public ValueTask PublishInventoryReserveRequestedAsync(Guid orderId, IReadOnlyCollection<InventoryItem> items)
    {
        return PublishAsync(new InventoryReserveRequestedEvent(orderId, items));
    }

    public ValueTask PublishOrderCompensatedAsync(Guid orderId, string reason)
    {
        return PublishAsync(new OrderCompensatedEvent(orderId, reason));
    }

    public ValueTask PublishOrderCompletedAsync(Guid orderId)
    {
        return PublishAsync(new OrderCompletedEvent(orderId));
    }

    public ValueTask PublishOrderCreatedAsync(Guid orderId, Guid customerId, decimal totalAmount)
    {
        return PublishAsync(new OrderCreatedEvent(orderId, customerId, totalAmount));
    }


    private async ValueTask PublishAsync<TEvent>(TEvent evt)
    {
        var channel = await _channelTask.Value.ConfigureAwait(false);
        var body = JsonSerializer.SerializeToUtf8Bytes(evt);
        var props = new BasicProperties
        {
            Persistent = true,
            Type = typeof(TEvent).Name
        };

        await channel.BasicPublishAsync(
            RabbitMQConstants.CheckoutExchange,
            RabbitMQConstants.CheckoutRoutingKey,
            true,
            props,
            body).ConfigureAwait(false);
    }
}