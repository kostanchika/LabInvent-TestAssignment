namespace DataProcessorService.Abstractions;

public interface IModuleProcesser
{
    Task ProcessAsync(string data, CancellationToken cancellationToken = default);
}
