using System.Xml;
using System.Xml.Serialization;
using XmlProcessingSystem.Shared.Models;

namespace FileParserService.Services;

public static class XmlInstrumentParser
{
    private static readonly Dictionary<string, Type> ModuleTypeMap = new()
    {
        ["SAMPLER"] = typeof(CombinedSamplerStatus),
        ["QUATPUMP"] = typeof(CombinedPumpStatus),
        ["COLCOMP"] = typeof(CombinedOvenStatus)
    };

    public static InstrumentStatus Parse(string path)
    {
        var doc = new XmlDocument();
        doc.Load(path);

        var root = doc.DocumentElement!;
        var instrument = new InstrumentStatus
        {
            PackageID = root["PackageID"]?.InnerText ?? throw new Exception("Could not find PackageID"),
            DeviceStatuses = []
        };

        foreach (XmlNode deviceNode in root.SelectNodes("DeviceStatus")!)
        {
            var categoryId = deviceNode["ModuleCategoryID"]?.InnerText;
            var index = int.Parse(deviceNode["IndexWithinRole"]?.InnerText ?? "0");

            var rapidNode = deviceNode["RapidControlStatus"];
            if (rapidNode == null || categoryId == null)
                continue;

            var moduleStatus = DeserializeEmbeddedXml(categoryId, rapidNode);

            instrument.DeviceStatuses.Add(new DeviceStatus
            {
                ModuleCategoryID = categoryId,
                IndexWithinRole = index,
                RapidControlStatus = moduleStatus
            });
        }

        return instrument;
    }

    private static ModuleStatus DeserializeEmbeddedXml(string categoryId, XmlNode rapidControlStatusNode)
    {
        if (!ModuleTypeMap.TryGetValue(categoryId, out var type))
            throw new NotSupportedException($"Unknown ModuleCategoryID: {categoryId}");

        var embeddedXml = rapidControlStatusNode.InnerText;

        // На всякий случай, удаление BOM (была ошибка)
        embeddedXml = embeddedXml.Trim('\uFEFF');

        var serializer = new XmlSerializer(type);
        using var reader = new StringReader(embeddedXml);
        return (ModuleStatus)serializer.Deserialize(reader)!;
    }
}
