using System.Xml.Serialization;

namespace XmlProcessingSystem.Shared.Models;

[XmlRoot("CombinedPumpStatus")]
public class CombinedPumpStatus : ModuleStatus
{
    public bool IsBusy { get; set; }
    public bool IsReady { get; set; }
    public bool IsError { get; set; }
    public bool KeyLock { get; set; }
    public string Mode { get; set; } = null!;
    public double Flow { get; set; }
    public double PercentB { get; set; }
    public double PercentC { get; set; }
    public double PercentD { get; set; }
    public double MinimumPressureLimit { get; set; }
    public double MaximumPressureLimit { get; set; }
    public double Pressure { get; set; }
    public bool PumpOn { get; set; }
    public int Channel { get; set; }
}
