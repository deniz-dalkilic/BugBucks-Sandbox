namespace BugBucks.Shared.Logging;

/// <summary>
///     Defines a logging abstraction for applications.
/// </summary>
/// <typeparam name="T">The type whose name is used as the logging category.</typeparam>
public interface IAppLogger<T>
{
    /// <summary>
    ///     Logs a trace message.
    /// </summary>
    /// <param name="message">The trace message template.</param>
    /// <param name="args">The arguments for formatting the message.</param>
    void LogTrace(string message, params object[] args);

    /// <summary>
    ///     Logs a debug message.
    /// </summary>
    /// <param name="message">The debug message template.</param>
    /// <param name="args">The arguments for formatting the message.</param>
    void LogDebug(string message, params object[] args);

    /// <summary>
    ///     Logs an informational message.
    /// </summary>
    /// <param name="message">The information message template.</param>
    /// <param name="args">The arguments for formatting the message.</param>
    void LogInformation(string message, params object[] args);

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message template.</param>
    /// <param name="args">The arguments for formatting the message.</param>
    void LogWarning(string message, params object[] args);

    /// <summary>
    ///     Logs an error message with an associated exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The error message template.</param>
    /// <param name="args">The arguments for formatting the message.</param>
    void LogError(Exception exception, string message, params object[] args);

    /// <summary>
    ///     Logs a fatal error with an associated exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The fatal error message template.</param>
    /// <param name="args">The arguments for formatting the message.</param>
    void LogFatal(Exception exception, string message, params object[] args);
}