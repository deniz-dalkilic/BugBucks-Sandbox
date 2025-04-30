using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record OrderCompletedEvent(Guid OrderId);
}
