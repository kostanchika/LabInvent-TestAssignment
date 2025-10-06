using System.Xml.Serialization;

namespace XmlProcessingSystem.Shared.Models;

[XmlRoot("CombinedOvenStatus")]
public class CombinedOvenStatus : ModuleStatus
{
    public bool IsBusy { get; set; }
    public bool IsReady { get; set; }
    public bool IsError { get; set; }
    public bool KeyLock { get; set; }
    public bool UseTemperatureControl { get; set; }
    public bool OvenOn { get; set; }
    public double Temperature_Actual { get; set; }
    public double Temperature_Room { get; set; }
    public double MaximumTemperatureLimit { get; set; }
    public int Valve_Position { get; set; }
    public int Valve_Rotations { get; set; }
    public bool Buzzer { get; set; }
}