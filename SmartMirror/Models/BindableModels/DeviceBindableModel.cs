using SmartMirror.Enums;
using SmartMirror.Resources;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.Models.BindableModels;

public class DeviceBindableModel : BindableBase
{
    #region -- Public properties --

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

    private string _positionId;
    public string PositionId
    {
        get => _positionId;
        set => SetProperty(ref _positionId, value);
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

    private string _iconSource = "grey_question_mark";
    public string IconSource
    {
        get => _iconSource;
        set => SetProperty(ref _iconSource, value);
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

    private string _additionalInfoFormatted;
    public string AdditionalInfoFormatted
    {
        get => GetAdditionalInfoFormatted();
        private set => SetProperty(ref _additionalInfoFormatted, value);
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

    private bool _isExecuting;
    public bool IsExecuting
    {
        get => _isExecuting;
        set => SetProperty(ref _isExecuting, value);
    }

    private string _editableResource;
    public string EditableResourceId
    {
        get => _editableResource;
        set => SetProperty(ref _editableResource, value);
    }

    private EUnitMeasure _unitMeasure;
    public EUnitMeasure UnitMeasure
    {
        get => _unitMeasure;
        set => SetProperty(ref _unitMeasure, value);
    }

    private bool _isShownInRooms = true;
    public bool IsShownInRooms
    {
        get => _isShownInRooms;
        set => SetProperty(ref _isShownInRooms, value);
    }

    private bool _isReceiveNotifications = true;
    public bool IsReceiveNotifications
    {
        get => _isReceiveNotifications;
        set => SetProperty(ref _isReceiveNotifications, value);
    }

    private bool _isFavorite;
    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    private ICommand _tappedCommand;
    public ICommand TappedCommand
    {
        get => _tappedCommand;
        set => SetProperty(ref _tappedCommand, value);
    }

    public bool HasEditableResource => !string.IsNullOrWhiteSpace(EditableResourceId);

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.PropertyName is nameof(AdditionalInfo) or nameof(IconSource) or nameof(State))
        {
            OnAdditionalInfoChanged();
        }
    }

    #endregion

    #region -- Private helpers --

    private void OnAdditionalInfoChanged()
    {
        AdditionalInfoFormatted = GetAdditionalInfoFormatted();
        Status = GetDeviceStatus();
    }

    private EDeviceStatus GetDeviceStatus()
    {
        var result = State == 0 ? EDeviceStatus.Disconnected : EDeviceStatus.Connected;

        //Temporally mock
        result = (DeviceType is EDeviceType.DoorbellStream or EDeviceType.DoorbellNoStream)
            ? EDeviceStatus.Connected
            : result;

        if (result is EDeviceStatus.Connected)
        {
            result = IconSource switch
            {
                IconsNames.pic_wall_switch_double_left => AdditionalInfo == "1" ? EDeviceStatus.On : EDeviceStatus.Off,
                IconsNames.pic_wall_switch_double_right => AdditionalInfo == "1" ? EDeviceStatus.On : EDeviceStatus.Off,
                IconsNames.pic_wall_switch_single => AdditionalInfo == "1" ? EDeviceStatus.On : EDeviceStatus.Off,
                IconsNames.pic_wall_switch_three_center => AdditionalInfo == "1" ? EDeviceStatus.On : EDeviceStatus.Off,
                IconsNames.pic_wall_switch_three_left => AdditionalInfo == "1" ? EDeviceStatus.On : EDeviceStatus.Off,
                IconsNames.pic_wall_switch_three_right => AdditionalInfo == "1" ? EDeviceStatus.On : EDeviceStatus.Off,
                IconsNames.pic_bell => State == 1 ? EDeviceStatus.On : EDeviceStatus.Off,
                _ => result,
            };
        }

        return result;
    }

    private string GetAdditionalInfoFormatted()
    {
        string result = null;

        if (!string.IsNullOrWhiteSpace(AdditionalInfo))
        {
            result = IconSource switch
            {
                IconsNames.pic_humidity => double.Parse(AdditionalInfo) / 100 + "%",
                IconsNames.pic_pressure => Math.Round(double.Parse(AdditionalInfo) / 1000, 2) + "kPa",
                IconsNames.pic_temperature => UnitMeasure == EUnitMeasure.Celsius ? double.Parse(AdditionalInfo) / 100 + "℃" : string.Format("{0:F2}", double.Parse(AdditionalInfo) / 100 * 1.8 + 32) + "℉",
                IconsNames.pic_wall_switch_double_left => "",
                IconsNames.pic_wall_switch_double_right => "",
                IconsNames.pic_wall_switch_single => "",
                IconsNames.pic_wall_switch_three_center => "",
                IconsNames.pic_wall_switch_three_left => "",
                IconsNames.pic_wall_switch_three_right => "",
                IconsNames.pic_motion => "",
                IconsNames.pic_dimmer => AdditionalInfo + " lux",
                _ => AdditionalInfo,
            };
        }

        return result;
    }

    #endregion
}

