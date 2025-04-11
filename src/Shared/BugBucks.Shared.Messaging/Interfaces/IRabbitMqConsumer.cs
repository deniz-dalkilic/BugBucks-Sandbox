namespace BugBucks.Shared.Messaging.Interfaces;

public interface IRabbitMqConsumer
{
    void StartConsuming(string queueName, Func<string, Task> onMessageReceived);
}