using System.Xml.Serialization;

namespace XmlProcessingSystem.Shared.Models;

[XmlRoot("CombinedSamplerStatus")]
public class CombinedSamplerStatus : ModuleStatus
{
    public bool IsBusy { get; set; }
    public bool IsReady { get; set; }
    public bool IsError { get; set; }
    public bool KeyLock { get; set; }
    public int Status { get; set; }
    public string Vial { get; set; } = null!;
    public double Volume { get; set; }
    public double MaximumInjectionVolume { get; set; }
    public string RackL { get; set; } = null!;
    public string RackR { get; set; } = null!;
    public int RackInf { get; set; }
    public bool Buzzer { get; set; }
}
