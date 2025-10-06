namespace XmlProcessingSystem.Shared.Models;

public class DeviceStatus
{
    public string ModuleCategoryID { get; set; } = null!;
    public int IndexWithinRole { get; set; }
    public ModuleStatus RapidControlStatus { get; set; } = null!;
}