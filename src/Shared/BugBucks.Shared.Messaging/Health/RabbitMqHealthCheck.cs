using BugBucks.Shared.Messaging.Infrastructure.RabbitMq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BugBucks.Shared.Messaging.Health;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnectionFactory _factory;

    public RabbitMqHealthCheck(IRabbitMqConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        try
        {
            using var conn = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
            if (conn.IsOpen)
                return HealthCheckResult.Healthy("RabbitMQ connection OK");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ unavailable", ex);
        }

        return HealthCheckResult.Unhealthy("RabbitMQ connection failed");
    }
}