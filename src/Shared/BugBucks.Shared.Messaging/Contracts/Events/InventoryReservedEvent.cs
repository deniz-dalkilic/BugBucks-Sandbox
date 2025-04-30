using System;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record InventoryReservedEvent(Guid OrderId);
}
