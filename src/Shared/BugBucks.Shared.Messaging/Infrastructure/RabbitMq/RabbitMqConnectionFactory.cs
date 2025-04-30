using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Infrastructure.RabbitMq;

public interface IRabbitMqConnectionFactory
{
    Task<IConnection> CreateConnectionAsync();
}

internal class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly ConnectionFactory _factory;

    public RabbitMqConnectionFactory(IConfiguration configuration)
    {
        var section = configuration.GetSection("RabbitMq");
        _factory = new ConnectionFactory
        {
            HostName = section["Host"],
            UserName = section["UserName"],
            Password = section["Password"],
            VirtualHost = section["VirtualHost"] ?? "/"
        };
    }

    public Task<IConnection> CreateConnectionAsync()
    {
        return _factory.CreateConnectionAsync();
    }
}