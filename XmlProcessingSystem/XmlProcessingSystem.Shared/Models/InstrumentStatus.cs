namespace XmlProcessingSystem.Shared.Models;

public class InstrumentStatus
{
    public string PackageID { get; set; } = null!;

    public List<DeviceStatus> DeviceStatuses { get; set; } = null!;
}
