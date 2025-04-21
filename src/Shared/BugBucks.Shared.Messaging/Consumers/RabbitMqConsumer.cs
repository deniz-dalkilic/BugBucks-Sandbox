using System.Text;
using BugBucks.Shared.Messaging.Connection;
using BugBucks.Shared.Messaging.Constants;
using BugBucks.Shared.Messaging.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BugBucks.Shared.Messaging.Consumers;

public class RabbitMqConsumer : IRabbitMqConsumer, IAsyncDisposable
{
    private readonly RabbitMqConnectionFactory _connectionFactory;
    private bool _disposed;

    private RabbitMqConsumer(RabbitMqConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await _connectionFactory.DisposeAsync();
        _disposed = true;
    }


    public void StartConsuming(string queueName, Func<string, Task> onMessageReceived)
    {
        var channel = _connectionFactory.GetChannel();

        channel.QueueDeclareAsync(
            queueName,
            RabbitMQConstants.Durable,
            RabbitMQConstants.Exclusive,
            RabbitMQConstants.AutoDelete);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await onMessageReceived(message);

            channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        channel.BasicConsumeAsync(queueName, false, consumer);
    }

    public void StartConsuming(string queueName)
    {
        throw new NotImplementedException();
    }

    public static async Task<RabbitMqConsumer> CreateAsync(
        string hostName,
        int port,
        string userName,
        string password,
        string virtualHost = "/",
        string? clientProvidedName = null,
        IEnumerable<AmqpTcpEndpoint>? endpoints = null)
    {
        var connectionFactory = await RabbitMqConnectionFactory.CreateAsync(hostName, port, userName, password,
            virtualHost, clientProvidedName, endpoints);
        return new RabbitMqConsumer(connectionFactory);
    }
}