namespace BugBucks.Shared.Messaging.Constants;

public static class RabbitMQConstants
{
    public const string DefaultExchange = "";
    public const bool Durable = true;
    public const bool Exclusive = false;
    public const bool AutoDelete = false;
    public const int MaxConcurrentCalls = 256;
}