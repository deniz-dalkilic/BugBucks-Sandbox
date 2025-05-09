using Microsoft.Extensions.Logging;

namespace BugBucks.Shared.Messaging.Retry;

public class MessageRetryPolicy
{
    public int MaxAttempts { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;

    public async Task ExecuteAsync(Func<Task> handler, ILogger logger, string messageType)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            try
            {
                await handler();
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[{MessageType}] Retry {Attempt}/{MaxAttempts} failed", messageType, attempt,
                    MaxAttempts);

                if (attempt == MaxAttempts)
                    throw;

                await Task.Delay(RetryDelayMs);
            }
    }
}