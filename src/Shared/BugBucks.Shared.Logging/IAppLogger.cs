namespace BugBucks.Shared.Logging;

/// <summary>
///     Defines a logging abstraction for applications.
/// </summary>
/// <typeparam name="T">The type whose name is used as the logging category.</typeparam>
public interface IAppLogger<T>
{
    void LogTrace(string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogFatal(Exception exception, string message, params object[] args);
}