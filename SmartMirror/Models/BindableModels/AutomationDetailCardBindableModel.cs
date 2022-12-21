using System.Collections.ObjectModel;

namespace SmartMirror.Models.BindableModels;

public class AutomationDetailCardBindableModel : BindableBase
{
    private string _deviceName;
    public string DeviceName
    {
        get => _deviceName;
        set => SetProperty(ref _deviceName, value);
    }

    private string _roomName;
    public string RoomName
    {
        get => _roomName;
        set => SetProperty(ref _roomName, value);
    }

    private string _iconSource;
    public string IconSource
    {
        get => _iconSource;
        set => SetProperty(ref _iconSource, value);
    }

    private string _condition;
    public string Condition
    {
        get => _condition;
        set => SetProperty(ref _condition, value);
    }

    private string _triggerName;
    public string TriggerName
    {
        get => _triggerName;
        set => SetProperty(ref _triggerName, value);
    }

    private DeviceBindableModel _device;
    public DeviceBindableModel Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }
}