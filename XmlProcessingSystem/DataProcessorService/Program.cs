using DataProcessorService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XmlProcessingSystem.Shared.Services;
using XmlProcessingSystem.Shared.Services.Configurations;

namespace DataProcessorService;

public sealed class Program
{
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath("/app")
            .AddEnvironmentVariables()
            .Build();

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
        var moduleRepository = new ModuleRepository(config["DB_CONNECTION"] ?? throw new Exception("DB_CONNECTION is not defined"));

        var moduleParser = new JsonModuleProcesser(moduleRepository, loggerFactory.CreateLogger<JsonModuleProcesser>());

        await messageBus.SubscribeAsync(async (message) =>
        {
            await moduleParser.ProcessAsync(message);
        });

        await Task.Delay(Timeout.Infinite);
    }
}
