namespace FileParserService.Abstractions;

public interface ISender
{
    Task SendAsync(string message, CancellationToken cancellationToken = default);
}
