namespace BugBucks.Shared.Logging.Config;

public class SeqOptions
{
    public bool Enabled { get; set; } = true;
    public string ServerUrl { get; set; } = string.Empty;

    public string MinimumLevel { get; set; } = "Information";
}