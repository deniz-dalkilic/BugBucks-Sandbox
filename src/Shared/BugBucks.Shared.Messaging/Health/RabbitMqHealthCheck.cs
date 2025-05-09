using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Health;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IConnectionFactory _factory;

    public RabbitMqHealthCheck(IConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        try
        {
            await using var conn = await _factory.CreateConnectionAsync(token);
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