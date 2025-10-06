using DataProcessorService.Models;

namespace DataProcessorService.Abstractions;

public interface IModuleRepository
{
    Task UpsertAsync(Module module, CancellationToken cancellationToken = default);
}
