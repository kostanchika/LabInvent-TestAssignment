using System.Text.Json;
using System.Text.Json.Serialization;
using XmlProcessingSystem.Shared.Models;

namespace FileParserService.Services;

public sealed class ModuleStatusJsonConverter : JsonConverter<ModuleStatus>
{
    public override void Write(Utf8JsonWriter writer, ModuleStatus value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

    public override ModuleStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
