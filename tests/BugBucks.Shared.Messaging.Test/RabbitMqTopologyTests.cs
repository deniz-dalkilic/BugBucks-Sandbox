using System.Text;
using BugBucks.Shared.Messaging.Contracts.Events;
using BugBucks.Shared.Messaging.Infrastructure.RabbitMq;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Tests;

public class RabbitMqTopologyTests
{
    private const string HostName = "localhost";
    private const int Port = 5672;

    [Fact]
    public async Task DeclareTopologyAsync_CreatesQueuesAndExchanges()
    {
        // RabbitMQ bağlantı fabrikası
        var factory = new ConnectionFactory
        {
            HostName = HostName,
            Port = Port,
            UserName = "guest",
            Password = "guest"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await RabbitMqTopology.DeclareAsync(channel);

        var testMessage = "TestMessage-" + Guid.NewGuid();
        var body = Encoding.UTF8.GetBytes(testMessage);
        var props = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            RabbitMqTopology.ExchangeName,
            "order.created",
            false,
            props,
            body
        );

        var getResult = await channel.BasicGetAsync(
            RabbitMqTopology.QueuePrefix + nameof(OrderCreatedEvent),
            true
        );

        Assert.NotNull(getResult);
        var received = Encoding.UTF8.GetString(getResult.Body.ToArray());
        Assert.Equal(testMessage, received);
    }
}