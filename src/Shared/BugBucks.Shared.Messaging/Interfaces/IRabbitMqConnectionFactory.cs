using RabbitMQ.Client;

namespace BugBucks.Shared.Messaging.Interfaces;

public interface IRabbitMqConnectionFactory : IAsyncDisposable
{
    IChannel GetChannel();
}