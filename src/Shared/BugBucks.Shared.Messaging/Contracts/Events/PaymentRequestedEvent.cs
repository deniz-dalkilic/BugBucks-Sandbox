using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record PaymentRequestedEvent(Guid OrderId, string PaymentDetails, string IdempotencyKey);
}
