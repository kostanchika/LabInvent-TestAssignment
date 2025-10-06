using XmlProcessingSystem.Shared.Enums;
namespace DataProcessorService.Models;

public sealed class Module
{
    public string ModuleCategoryId { get; set; }
    public ModuleState ModuleState { get; set; }
}
