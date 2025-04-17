namespace BugBucks.Shared.Logging.Config;

public class ElasticsearchOptions
{
    public bool Enabled { get; set; } = true;
    public string[] NodeUris { get; set; } = Array.Empty<string>();
    public DataStreamOptions DataStream { get; set; } = new();
    public string BootstrapMethod { get; set; } = "Failure";
    public BufferOptionsConfig BufferOptions { get; set; } = new();
}