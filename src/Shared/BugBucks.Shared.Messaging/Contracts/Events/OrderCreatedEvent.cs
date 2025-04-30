using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount);
}
