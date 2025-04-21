using System.Text;
using BugBucks.Shared.Messaging.Connection;
using BugBucks.Shared.Messaging.Constants;
using BugBucks.Shared.Messaging.Interfaces;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Publishers;

public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly RabbitMqConnectionFactory _connectionFactory;
    private bool _disposed;

    private RabbitMqPublisher(RabbitMqConnectionFactory connectionFactory)
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

    public Task PublishAsync(string queueName, string message)
    {
        var channel = _connectionFactory.GetChannel();

        channel.QueueDeclareAsync(
            queueName,
            RabbitMQConstants.Durable,
            RabbitMQConstants.Exclusive,
            RabbitMQConstants.AutoDelete);

        var body = Encoding.UTF8.GetBytes(message);

        var basicProperties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent
        };

        channel.BasicPublishAsync(
            RabbitMQConstants.DefaultExchange,
            queueName,
            true,
            basicProperties,
            body);

        return Task.CompletedTask;
    }

    public static async Task<RabbitMqPublisher> CreateAsync(
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
        return new RabbitMqPublisher(connectionFactory);
    }
}