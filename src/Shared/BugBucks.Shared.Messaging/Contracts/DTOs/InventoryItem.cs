using System;

namespace BugBucks.Shared.Messaging.Contracts.DTOs
{
    public record InventoryItem(Guid ProductId, int Quantity);
}
