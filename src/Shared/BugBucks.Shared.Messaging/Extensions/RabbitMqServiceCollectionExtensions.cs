using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace BugBucks.Shared.Messaging.Extensions;

public static class RabbitMqServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("RabbitMq");

        var factory = new ConnectionFactory
        {
            HostName = section["Host"],
            UserName = section["Username"],
            Password = section["Password"],
            VirtualHost = section["VirtualHost"] ?? "/"
        };

        services.AddSingleton<IConnection>(sp =>
        {
            var retryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .WaitAndRetry(
                    5,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                );

            return retryPolicy.Execute(() =>
                factory
                    .CreateConnectionAsync()
                    .GetAwaiter()
                    .GetResult());
        });

        // Declare topology at startup
        //services.AddHostedService<RabbitMqTopologyInitializer>();

        return services;
    }
}