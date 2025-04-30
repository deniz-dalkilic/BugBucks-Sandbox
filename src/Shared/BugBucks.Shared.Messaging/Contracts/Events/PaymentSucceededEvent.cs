using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record PaymentSucceededEvent(Guid OrderId, string TransactionId);
}
