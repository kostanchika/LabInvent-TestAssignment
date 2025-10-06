using FileParserService.Services;
using FileParserService.Services.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XmlProcessingSystem.Shared.Services;
using XmlProcessingSystem.Shared.Services.Configurations;

namespace FileParserService;

public sealed class Program
{
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath("/app")
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var fileMonitorSettings = new FileMonitorConfiguration()
        {
            MonitoringDelayMiliseconds = int.Parse(config["Monitor:WatchDelayMiliseconds"]
                ?? throw new Exception("Monitor:WatchDelayMiliseconds is not defined")),
            WatchingDirectory = config["Monitor:WatchDirectory"] ?? throw new Exception("Monitor:WatchDirectory is not defined")
        };

        var rabbitMQSettings = new RabbitMQSettings
        {
            Host = config["RABBITMQ_HOST"] ?? throw new Exception("RABBITMQ_HOST is not defined"),
            UserName = config["RABBITMQ_USER"] ?? throw new Exception("RABBITMQ_USER is not defined"),
            Password = config["RABBITMQ_PASSWORD"] ?? throw new Exception("RABBITMQ_PASSWORD is not defined"),
            Port = int.Parse(config["RABBITMQ_PORT"] ?? throw new Exception("RABBITMQ_PORT is not defined")),
            QueueName = config["RABBITMQ_QUEUE"] ?? throw new Exception("RABBITMQ_QUEUE is not defined")
        };

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });

        var messageBus = await RabbitMQMessageBus.CreateAsync(rabbitMQSettings, loggerFactory.CreateLogger<RabbitMQMessageBus>());
        var sender = new Sender(messageBus);
        var xmlProcessor = new XmlProcesser(sender, loggerFactory.CreateLogger<XmlProcesser>());
        var fileMonitor = new FileMonitor(
            fileMonitorSettings,
            xmlProcessor,
            loggerFactory.CreateLogger<FileMonitor>()
        );

        await fileMonitor.StartAsync();
    }
}
