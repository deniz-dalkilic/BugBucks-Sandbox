namespace BugBucks.Shared.Logging.Config;

public class OpenTelemetryOptions
{
    public string OtlpEndpoint { get; set; } = string.Empty;
    public LoggingOptions Logging { get; set; } = new();

    public class LoggingOptions
    {
        public bool Enabled { get; set; } = true;
        public string MinimumLevel { get; set; } = "Information";
    }
}