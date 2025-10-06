using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using XmlProcessingSystem.Shared.Abstractions;
using XmlProcessingSystem.Shared.Services.Configurations;

namespace XmlProcessingSystem.Shared.Services;

public sealed class RabbitMQMessageBus : IMessageBus
{
    private readonly ILogger<RabbitMQMessageBus> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;

    private RabbitMQMessageBus(ILogger<RabbitMQMessageBus> logger, IConnection connection, IChannel channel, string queueNmae)
    {
        _logger = logger;
        _connection = connection;
        _channel = channel;
        _queueName = queueNmae;
    }

    public static async Task<RabbitMQMessageBus> CreateAsync(RabbitMQSettings settings, ILogger<RabbitMQMessageBus> logger)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            ConsumerDispatchConcurrency = 10
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var bus = new RabbitMQMessageBus(logger, connection, channel, settings.QueueName);

        await bus.CheckAvailabilityAsync();

        return bus;

    }

    public async Task PublishAsync(string message, CancellationToken cancellationToken = default)
    {
        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync("", _queueName, body, cancellationToken);

        _logger.LogInformation("Published message to queue {Queue}", _queueName);
    }

    public async Task SubscribeAsync(Func<string, Task> onMessage)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message from queue {Queue}", _queueName);

                await onMessage(message);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        };

        await _channel.BasicConsumeAsync(_queueName, false, consumer);
    }

    private async Task<bool> CheckAvailabilityAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _channel.QueueDeclarePassiveAsync(_queueName, cancellationToken);

            _logger.LogInformation("RabbitMQ queue {Queue} is OK. Message count: {Count}", _queueName, result.MessageCount);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ queue {Queue} is unavailable", _queueName);

            throw;
        }
    }
}
