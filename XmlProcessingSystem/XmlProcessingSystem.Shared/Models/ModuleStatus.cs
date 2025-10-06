using XmlProcessingSystem.Shared.Enums;

namespace XmlProcessingSystem.Shared.Models;

public abstract class ModuleStatus
{
    public ModuleState ModuleState { get; set; }
}
