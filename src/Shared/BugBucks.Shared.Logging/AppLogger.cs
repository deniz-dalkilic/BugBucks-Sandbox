using Microsoft.Extensions.Logging;

namespace BugBucks.Shared.Logging;

/// <summary>
///     A concrete implementation of the IAppLogger interface using Microsoft.Extensions.Logging.
/// </summary>
/// <typeparam name="T">The type whose name is used as the logging category.</typeparam>
public class AppLogger<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    /// <summary>
    ///     Initializes a new instance of the AppLogger class.
    /// </summary>
    /// <param name="logger">The Microsoft ILogger instance injected via DI.</param>
    public AppLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void LogTrace(string message, params object[] args)
    {
        _logger.LogTrace(message, args);
    }

    /// <inheritdoc />
    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    /// <inheritdoc />
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    /// <inheritdoc />
    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    /// <inheritdoc />
    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    /// <inheritdoc />
    public void LogFatal(Exception exception, string message, params object[] args)
    {
        // Using LogCritical to represent fatal errors.
        _logger.LogCritical(exception, message, args);
    }
}