namespace FileParserService.Services.Configurations;

public sealed class FileMonitorConfiguration
{
    public int MonitoringDelayMiliseconds { get; set; }
    public string WatchingDirectory { get; set; } = null!;
}
