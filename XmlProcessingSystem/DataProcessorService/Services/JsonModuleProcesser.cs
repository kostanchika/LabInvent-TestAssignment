using DataProcessorService.Abstractions;
using DataProcessorService.Models;
using Microsoft.Extensions.Logging;

namespace DataProcessorService.Services;

public sealed class JsonModuleProcesser : IModuleProcesser
{
    private readonly IModuleRepository _moduleRepository;
    private readonly ILogger<JsonModuleProcesser> _logger;
    public JsonModuleProcesser(IModuleRepository moduleRepository, ILogger<JsonModuleProcesser> logger)
    {
        _moduleRepository = moduleRepository;
        _logger = logger;
    }

    public async Task ProcessAsync(string json, CancellationToken cancellationToken = default)
    {
        var instrument = JsonInstrumentParser.Parse(json);

        foreach (var device in instrument.DeviceStatuses)
        {
            var module = new Module()
            {
                ModuleCategoryId = device.ModuleCategoryID,
                ModuleState = device.RapidControlStatus.ModuleState
            };

            await _moduleRepository.UpsertAsync(module, cancellationToken);
        }

        _logger.LogInformation("Successfully proccesed instrument with PackageID={PackageID}", instrument.PackageID);
    }
}
