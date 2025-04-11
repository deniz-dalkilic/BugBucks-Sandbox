using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Implementations;

public class RabbitMqConnectionFactory : IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly IConnection _connection;
    private bool _disposed;

    private RabbitMqConnectionFactory(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _disposed = true;
    }

    public static async Task<RabbitMqConnectionFactory> CreateAsync(
        string hostName,
        int port,
        string userName,
        string password,
        string virtualHost = "/",
        string clientProvidedName = null,
        IEnumerable<AmqpTcpEndpoint> endpoints = null)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            VirtualHost = virtualHost
        };

        if (!string.IsNullOrWhiteSpace(clientProvidedName))
            factory.ClientProvidedName = clientProvidedName;

        var connection = endpoints is not null
            ? await factory.CreateConnectionAsync(new List<AmqpTcpEndpoint>(endpoints))
            : await factory.CreateConnectionAsync();

        var channel = await connection.CreateChannelAsync();

        return new RabbitMqConnectionFactory(connection, channel);
    }

    public IChannel GetChannel()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(RabbitMqConnectionFactory));

        return _channel;
    }
}