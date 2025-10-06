namespace XmlProcessingSystem.Shared.Abstractions;

public interface IMessageBus
{
    Task PublishAsync(string message, CancellationToken cancellationToken = default);
    Task SubscribeAsync(Func<string, Task> onMessage);
}
