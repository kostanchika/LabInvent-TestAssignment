using System.Text.Json;
using XmlProcessingSystem.Shared.Models;

namespace DataProcessorService.Services;

public static class JsonInstrumentParser
{
    private static readonly Dictionary<string, Type> ModuleTypeMap = new()
    {
        ["SAMPLER"] = typeof(CombinedSamplerStatus),
        ["QUATPUMP"] = typeof(CombinedPumpStatus),
        ["COLCOMP"] = typeof(CombinedOvenStatus)
    };

    public static InstrumentStatus Parse(string json)
    {
        var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        var instrument = new InstrumentStatus
        {
            PackageID = root.GetProperty("PackageID").GetString() ?? throw new Exception("Could not find PackageID"),
            DeviceStatuses = []
        };

        foreach (var deviceJson in root.GetProperty("DeviceStatuses").EnumerateArray())
        {
            var categoryId = deviceJson.GetProperty("ModuleCategoryID").GetString();
            var index = deviceJson.GetProperty("IndexWithinRole").GetInt32();
            var rapidJson = deviceJson.GetProperty("RapidControlStatus");

            if (categoryId == null)
            {
                throw new Exception("Could not find ModuleCategoryID");
            }

            var moduleStatus = DeserializeModuleStatus(categoryId, rapidJson);

            instrument.DeviceStatuses.Add(new DeviceStatus
            {
                ModuleCategoryID = categoryId,
                IndexWithinRole = index,
                RapidControlStatus = moduleStatus
            });
        }

        return instrument;
    }

    private static ModuleStatus DeserializeModuleStatus(string categoryId, JsonElement element)
    {
        if (!ModuleTypeMap.TryGetValue(categoryId, out var type))
            throw new NotSupportedException($"Unknown ModuleCategoryID: {categoryId}");

        var json = element.GetRawText();
        return (ModuleStatus)JsonSerializer.Deserialize(json, type)!;
    }
}