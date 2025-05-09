using BugBucks.Shared.Messaging.Abstractions.Messaging;
using BugBucks.Shared.Messaging.Infrastructure.RabbitMq;
using BugBucks.Shared.Messaging.Retry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BugBucks.Shared.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<MessageRetryPolicy>();
        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
        return services;
    }

    public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services,
        IConfiguration configuration)
    {
        return AddSharedMessaging(services, configuration);
    }
}