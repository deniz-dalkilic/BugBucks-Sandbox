using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record PaymentFailedEvent(Guid OrderId, string ErrorCode);
}
