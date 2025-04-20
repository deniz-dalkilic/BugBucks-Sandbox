//using BugBucks.Shared.Messaging.Implementations;
//using BugBucks.Shared.Messaging.Interfaces;

namespace BugBucks.Shared.Messaging.Tests;

public class RabbitMqTests : IAsyncLifetime
{
    /*
    private IRabbitMqConsumer _consumer;
    private IRabbitMqPublisher _publisher;

    public async Task InitializeAsync()
    {
        var hostName = "localhost";
        var port = 5672;
        var userName = "guest";
        var password = "guest";

        _publisher = await RabbitMqPublisher.CreateAsync(hostName, port, userName, password);
        _consumer = await RabbitMqConsumer.CreateAsync(hostName, port, userName, password);
    }

    public async Task DisposeAsync()
    {
        if (_publisher is IAsyncDisposable publisherDisposable)
            await publisherDisposable.DisposeAsync();
        if (_consumer is IAsyncDisposable consumerDisposable)
            await consumerDisposable.DisposeAsync();
    }

    [Fact]
    public async Task PublishMessage_ShouldSucceed()
    {
        var queueName = "test-queue";
        var testMessage = "Hello, RabbitMQ!";
        var messageReceived = false;
        var tcs = new TaskCompletionSource<bool>();

        await _publisher.PublishAsync(queueName, testMessage);

        _consumer.StartConsuming(queueName, async msg =>
        {
            if (msg == testMessage)
            {
                messageReceived = true;
                tcs.SetResult(true);
            }

            await Task.CompletedTask;
        });

        await Task.WhenAny(tcs.Task, Task.Delay(5000));


        Assert.True(messageReceived, "Published message was not received within the timeout period.");
    }*/
    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task DisposeAsync()
    {
        throw new NotImplementedException();
    }
}