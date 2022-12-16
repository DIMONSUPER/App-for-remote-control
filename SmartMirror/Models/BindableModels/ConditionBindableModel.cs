using SmartMirror.Models.Aqara;

namespace SmartMirror.Models.BindableModels;

public class ConditionBindableModel : BindableBase
{
    private string _triggerDefinitionId;
    public string TriggerDefinitionId
    {
        get => _triggerDefinitionId;
        set => SetProperty(ref _triggerDefinitionId, value);
    }

    private string _triggerName;
    public string TriggerName
    {
        get => _triggerName;
        set => SetProperty(ref _triggerName, value);
    }

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

    private string _subjectId;
    public string SubjectId
    {
        get => _subjectId;
        set => SetProperty(ref _subjectId, value);
    }

    private string _model;
    public string Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    private string _beginTime;
    public string BeginTime
    {
        get => _beginTime;
        set => SetProperty(ref _beginTime, value);
    }

    private string _endTime;
    public string EndTime
    {
        get => _endTime;
        set => SetProperty(ref _endTime, value);
    }

    private List<ParamAqaraModel> _params;
    public List<ParamAqaraModel> Params
    {
        get => _params;
        set => SetProperty(ref _params, value);
    }

    private string _condition;
    public string Condition
    {
        get => _condition;
        set => SetProperty(ref _condition, value);
    }

    private DeviceBindableModel _device;
    public DeviceBindableModel Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }
}

