using FileParserService.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using XmlProcessingSystem.Shared.Enums;

namespace FileParserService.Services;

public sealed class XmlProcesser : IXmlProcesser
{
    private readonly ISender _sender;
    private readonly ILogger<XmlProcesser> _logger;
    private readonly Random _random = new();
    private readonly Array _values = Enum.GetValues(typeof(ModuleState));
    private readonly JsonSerializerOptions _options = new() { Converters = { new ModuleStatusJsonConverter() } };

    public XmlProcesser(ISender sender, ILogger<XmlProcesser> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task ProcessFileAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var instrumentStatus = XmlInstrumentParser.Parse(path);

            foreach (var device in instrumentStatus.DeviceStatuses)
            {
                device.RapidControlStatus.ModuleState = (ModuleState)_values.GetValue(_random.Next(_values.Length))!;
            }

            var json = JsonSerializer.Serialize(instrumentStatus, _options);
            await _sender.SendAsync(json, cancellationToken);
            _logger.LogInformation("Processed: {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not process file {Path}: {Exception}", path, ex);
        }
    }
}
