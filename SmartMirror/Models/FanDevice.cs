namespace SmartMirror.Models;

public class FanDevice : Device
{
    public FanDevice() : base()
    {
        DeviceType = Enums.EDeviceType.Switcher;
    }

    public string AdditionalInfo { get; set; }
}

