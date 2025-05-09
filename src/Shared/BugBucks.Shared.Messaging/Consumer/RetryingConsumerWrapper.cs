using BugBucks.Shared.Messaging.Retry;
using Microsoft.Extensions.Logging;

namespace BugBucks.Shared.Messaging.Consumer;

public class RetryingConsumerWrapper
{
    private readonly ILogger _logger;
    private readonly MessageRetryPolicy _policy;

    public RetryingConsumerWrapper(MessageRetryPolicy policy, ILogger logger)
    {
        _policy = policy;
        _logger = logger;
    }

    public async Task HandleAsync(string messageType, Func<Task> handler)
    {
        await _policy.ExecuteAsync(handler, _logger, messageType);
    }
}