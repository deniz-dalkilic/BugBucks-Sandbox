using BugBucks.Shared.Messaging.Interfaces;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Connection;

public sealed class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly IConnection _connection;
    private bool _disposed;

    private RabbitMqConnectionFactory(IConnection connection, IChannel channel)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_channel?.IsOpen == true) await _channel.CloseAsync().ConfigureAwait(false);
            _channel?.Dispose();

            if (_connection?.IsOpen == true) await _connection.CloseAsync().ConfigureAwait(false);
            _connection?.Dispose();
        }
        finally
        {
            _disposed = true;
        }
    }

    public IChannel GetChannel()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(RabbitMqConnectionFactory));

        if (_channel?.IsOpen != true) throw new InvalidOperationException("Channel is not available or not open.");

        return _channel;
    }

    public static async Task<RabbitMqConnectionFactory> CreateAsync(
        string hostName,
        int port,
        string userName,
        string password,
        string virtualHost = "/",
        string? clientProvidedName = null,
        IEnumerable<AmqpTcpEndpoint>? endpoints = null)
    {
        if (string.IsNullOrWhiteSpace(hostName))
            throw new ArgumentException("Host name cannot be null or empty.", nameof(hostName));
        if (port <= 0)
            throw new ArgumentOutOfRangeException(nameof(port), "Port must be positive.");
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            VirtualHost = virtualHost
        };

        if (!string.IsNullOrWhiteSpace(clientProvidedName)) factory.ClientProvidedName = clientProvidedName;

        var connection = endpoints is not null
            ? await factory.CreateConnectionAsync(new List<AmqpTcpEndpoint>(endpoints)).ConfigureAwait(false)
            : await factory.CreateConnectionAsync().ConfigureAwait(false);

        var channel = await connection.CreateChannelAsync().ConfigureAwait(false);

        return new RabbitMqConnectionFactory(connection, channel);
    }
}