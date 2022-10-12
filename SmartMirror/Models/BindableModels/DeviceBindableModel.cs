using System.Windows.Input;
using SmartMirror.Enums;

namespace SmartMirror.Models.BindableModels;

public class DeviceBindableModel : BindableBase
{
    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _deviceId;
    public string DeviceId
    {
        get => _deviceId;
        set => SetProperty(ref _deviceId, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private EDeviceStatus _status;
    public EDeviceStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    private EDeviceType _deviceType;
    public EDeviceType DeviceType
    {
        get => _deviceType;
        set => SetProperty(ref _deviceType, value);
    }

    private string _type;
    public string Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    private string _roomName;
    public string RoomName
    {
        get => _roomName;
        set => SetProperty(ref _roomName, value);
    }

    private string _additionalInfo;
    public string AdditionalInfo
    {
        get => _additionalInfo;
        set => SetProperty(ref _additionalInfo, value);
    }

    private string _parentDid;
    public string ParentDid
    {
        get => _parentDid;
        set => SetProperty(ref _parentDid, value);
    }

    private DateTime _createTime;
    public DateTime CreateTime
    {
        get => _createTime;
        set => SetProperty(ref _createTime, value);
    }

    private string _timeZone;
    public string TimeZone
    {
        get => _timeZone;
        set => SetProperty(ref _timeZone, value);
    }

    private string _model;
    public string Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    private DateTime _updateTime;
    public DateTime UpdateTime
    {
        get => _updateTime;
        set => SetProperty(ref _updateTime, value);
    }

    private int _modelType;
    public int ModelType
    {
        get => _modelType;
        set => SetProperty(ref _modelType, value);
    }

    private int _state;
    public int State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }

    private ICommand _tappedCommand;
    public ICommand TappedCommand
    {
        get => _tappedCommand;
        set => SetProperty(ref _tappedCommand, value);
    }
}

