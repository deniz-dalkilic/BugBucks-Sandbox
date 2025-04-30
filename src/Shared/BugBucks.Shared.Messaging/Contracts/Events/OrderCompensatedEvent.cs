using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record OrderCompensatedEvent(Guid OrderId, string Reason);
}
