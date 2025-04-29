using System.Text;
using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Messaging.Constants;
using BugBucks.Shared.Messaging.Topology;
using CheckoutService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace CheckoutService.Application.Services;

/// <summary>
///     Background service that publishes pending outbox messages to RabbitMQ.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IAppLogger<OutboxProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, IConnection connection,
        IAppLogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        // use a direct channel for pub/sub
        _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.DeclareCheckoutTopologyAsync().GetAwaiter().GetResult();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CheckoutSagaDbContext>();

            var pending = db.OutboxMessages
                .Where(x => !x.Processed)
                .OrderBy(x => x.OccurredAt)
                .Take(20)
                .ToList();

            _logger.LogDebug("OutboxProcessor: found {Count} messages", pending.Count);

            foreach (var msg in pending)
                try
                {
                    _logger.LogDebug("Publishing outbox message {Id} of type {Type}", msg.Id, msg.Type);

                    var props = new BasicProperties();
                    props.Persistent = true;
                    props.Type = msg.Type;
                    props.MessageId = msg.Id.ToString();

                    var body = Encoding.UTF8.GetBytes(msg.Content);

                    await _channel.BasicPublishAsync(
                            RabbitMQConstants.CheckoutExchange,
                            RabbitMQConstants.CheckoutRoutingKey,
                            true,
                            props,
                            body)
                        .ConfigureAwait(false);

                    msg.Processed = true;
                    msg.ProcessedAt = DateTime.UtcNow;
                    db.Update(msg);

                    _logger.LogDebug("Marking outbox message {Id} processed", msg.Id);

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}