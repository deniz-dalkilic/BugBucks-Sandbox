namespace BugBucks.Shared.Messaging.Constants;

public static class RabbitMQConstants
{
    public const string DefaultExchange = "default.exchange.key";
    public const string DefaultRoutingKey = "default.routing.key";
    public const bool Durable = true;
    public const bool Exclusive = false;
    public const bool AutoDelete = false;
    public const int MaxConcurrentCalls = 256;

    // Exchanges
    public const string CheckoutExchange = "checkout.exchange";
    public const string CheckoutDeadLetterExchange = "checkout.dlx";

    // Queues
    public const string CheckoutQueue = "checkout.queue";
    public const string CheckoutDeadLetterQueue = "checkout.dlq";

    // Routing Keys
    public const string CheckoutRoutingKey = "checkout.key";
}