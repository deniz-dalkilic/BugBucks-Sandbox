using System.Text.Json;
using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Retry;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BugBucks.Shared.Messaging.Infrastructure.RabbitMq;

internal class RabbitMqConsumer : IMessageConsumer
{
    private readonly IRabbitMqConnectionFactory _factory;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly MessageRetryPolicy _retryPolicy;

    public RabbitMqConsumer(IRabbitMqConnectionFactory factory, MessageRetryPolicy retryPolicy,
        ILogger<RabbitMqConsumer> logger)
    {
        _factory = factory;
        _retryPolicy = retryPolicy;
        _logger = logger;
    }

    public async Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var connection = await _factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await RabbitMqTopology.DeclareAsync(channel);

        var queueName = RabbitMqTopology.QueuePrefix + typeof(TEvent).Name;
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var evt = JsonSerializer.Deserialize<TEvent>(body);
            var tag = ea.DeliveryTag;

            if (evt is null)
            {
                _logger.LogWarning("Received null payload for {Event}", typeof(TEvent).Name);
                await channel.BasicNackAsync(tag, false, false);
                return;
            }

            try
            {
                await _retryPolicy.ExecuteAsync(
                    () => handler(evt),
                    _logger,
                    typeof(TEvent).Name
                );


                await channel.BasicAckAsync(tag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message processing failed irrecoverably, nacking to DLQ");
                await channel.BasicNackAsync(tag, false, false);
            }
        };


        await channel.BasicConsumeAsync(queueName, false, consumer);
    }
}