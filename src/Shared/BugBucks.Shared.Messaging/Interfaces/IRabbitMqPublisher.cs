namespace BugBucks.Shared.Messaging.Interfaces;

public interface IRabbitMqPublisher
{
    Task PublishAsync(string queueName, string message);
}