using System.Text;
using BugBucks.Shared.Messaging.Constants;
using BugBucks.Shared.Messaging.Topology;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Tests;

public class RabbitMqTopologyTests
{
    private const string HostName = "localhost";
    private const int Port = 5672;

    [Fact]
    public async Task DeclareCheckoutTopologyAsync_CreatesQueuesAndExchanges()
    {
        var factory = new ConnectionFactory
        {
            HostName = HostName,
            Port = Port,
            UserName = "guest",
            Password = "guest"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.DeclareCheckoutTopologyAsync();

        var testMessage = DateTime.UtcNow.ToString("O");
        var body = Encoding.UTF8.GetBytes(testMessage);
        var props = new BasicProperties();
        props.DeliveryMode = DeliveryModes.Persistent;

        await channel.BasicPublishAsync(
            RabbitMQConstants.CheckoutExchange,
            RabbitMQConstants.CheckoutRoutingKey,
            true,
            props,
            body
        );


        var getResult = await channel.BasicGetAsync(
            RabbitMQConstants.CheckoutQueue,
            true
        );


        Assert.NotNull(getResult);
        var received = Encoding.UTF8.GetString(getResult.Body.ToArray());
        Assert.Equal(testMessage, received);
    }
}