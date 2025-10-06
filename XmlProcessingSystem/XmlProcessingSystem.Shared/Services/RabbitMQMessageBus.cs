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
    private readonly IChannel _channel;
    private readonly string _queueName;

    private RabbitMQMessageBus(ILogger<RabbitMQMessageBus> logger, IChannel channel, string queueNmae)
    {
        _logger = logger;
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

        var bus = new RabbitMQMessageBus(logger, channel, settings.QueueName);

        return bus;
    }

    public async Task PublishAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_channel == null || !_channel.IsOpen)
        {
            throw new InvalidOperationException("RabbitMQ channel is not open.");
        }


        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync("", _queueName, body, cancellationToken);

        _logger.LogInformation("Published message to queue {Queue}", _queueName);
    }

    public async Task SubscribeAsync(Func<string, Task> onMessage)
    {
        if (_channel == null || !_channel.IsOpen)
        {
            throw new InvalidOperationException("RabbitMQ channel is not open.");
        }

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
}
