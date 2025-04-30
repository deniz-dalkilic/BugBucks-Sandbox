using System;
using System.Collections.Generic;
using BugBucks.Shared.Messaging.Contracts.DTOs;

namespace BugBucks.Shared.Messaging.Contracts.Events
{
    public record InventoryReserveRequestedEvent(Guid OrderId, IReadOnlyCollection<InventoryItem> Items);
}
