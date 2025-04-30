using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record InventoryFailedEvent(Guid OrderId, string Reason);
}
