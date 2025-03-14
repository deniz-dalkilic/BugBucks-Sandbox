namespace LoggingService.Api.Configurations;

public class ElasticsearchOptions
{
    public string[] NodeUris { get; set; }
    public DataStreamOptions DataStream { get; set; }
    public string BootstrapMethod { get; set; }
    public BufferOptionsConfig BufferOptions { get; set; }
}

public class DataStreamOptions
{
    public string Type { get; set; }
    public string Dataset { get; set; }
    public string Namespace { get; set; }
}

public class BufferOptionsConfig
{
    public int ExportMaxConcurrency { get; set; }
}