using FileParserService.Services;
using FileParserService.Services.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XmlProcessingSystem.Shared.Services;
using XmlProcessingSystem.Shared.Services.Configurations;

namespace FileParserService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var fileMonitorSettings = new FileMonitorConfiguration()
        {
            MonitoringDelayMiliseconds = int.Parse(config["WatchDelayMiliseconds"]),
            WatchingDirectory = config["Monitor:WatchDirectory"]
        };

        var rabbitMQSettings = new RabbitMQSettings
        {
            Host = config["RABBITMQ_HOST"],
            UserName = config["RABBITMQ_USER"],
            Password = config["RABBITMQ_PASSWORD"],
            Port = int.Parse(config["RABBITMQ_PORT"]),
            QueueName = config["RABBITMQ_QUEUE"]
        };

        using var loggerFactory = LoggerFactory.Create(builder => 
        {

            builder.AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });

        var messageBuss = new RabbitMQMessageBus()

        var sender = new Sender()

        var xmlProcessor = new XmlProcesser()

        var fileMonitor = new FileMonitor(
            fileMonitorSettings,
            
        )
    }
}
