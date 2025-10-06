using FileParserService.Abstractions;
using FileParserService.Services.Configurations;
using Microsoft.Extensions.Logging;

namespace FileParserService.Services;

public sealed class FileMonitor
{
    private readonly IXmlProcesser _processer;
    private readonly FileMonitorConfiguration _configuration;
    private readonly ILogger<FileMonitor> _logger;

    public FileMonitor(FileMonitorConfiguration configuration, IXmlProcesser processer, ILogger<FileMonitor> logger)
    {
        _configuration = configuration;
        _processer = processer;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var files = Directory.GetFiles(_configuration.WatchingDirectory, "*.xml");

            _logger.LogInformation(
                "Start watching directory {Directory}, found {Count} files",
                _configuration.WatchingDirectory,
                files.Length
            );

            foreach (var file in files)
            {
                _ = Task.Run(() => _processer.ProcessFileAsync(file), cancellationToken);
            }

            await Task.Delay(_configuration.MonitoringDelayMiliseconds, cancellationToken);
        }
    }
}
