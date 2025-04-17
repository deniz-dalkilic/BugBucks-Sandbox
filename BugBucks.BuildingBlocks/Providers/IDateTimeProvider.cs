namespace BugBucks.BuildingBlocks.Providers;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}