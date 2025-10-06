using FileParserService.Abstractions;
using XmlProcessingSystem.Shared.Abstractions;

namespace FileParserService.Services;

public sealed class Sender : ISender
{
    private readonly IMessageBus _messageBus;

    public Sender(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public Task SendAsync(string json, CancellationToken cancellationToken = default)
    {
        return _messageBus.PublishAsync(json, cancellationToken);
    }
}
