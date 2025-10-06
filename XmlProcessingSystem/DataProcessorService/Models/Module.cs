using XmlProcessingSystem.Shared.Enums;
namespace DataProcessorService.Models;

public sealed class Module
{
    public string ModuleCategoryId { get; set; } = null!;
    public ModuleState ModuleState { get; set; }
}
