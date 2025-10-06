namespace FileParserService.Abstractions;

public interface IXmlProcesser
{
    Task ProcessFileAsync(string path, CancellationToken cancellationToken = default);
}
