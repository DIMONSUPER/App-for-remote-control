using System.Collections.ObjectModel;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Models.BindableModels;

public class ActionBindableModel : BindableBase
{
    private string _subjectId;
    public string SubjectId
    {
        get => _subjectId;
        set => SetProperty(ref _subjectId, value);
    }

    private string _actionName;
    public string ActionName
    {
        get => _actionName;
        set => SetProperty(ref _actionName, value);
    }

    private string _model;
    public string Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    private string _delayTime;
    public string DelayTime
    {
        get => _delayTime;
        set => SetProperty(ref _delayTime, value);
    }

    private string _delayTimeUnit;
    public string DelayTimeUnit
    {
        get => _delayTimeUnit;
        set => SetProperty(ref _delayTimeUnit, value);
    }

    private string _actionDefinitionId;
    public string ActionDefinitionId
    {
        get => _actionDefinitionId;
        set => SetProperty(ref _actionDefinitionId, value);
    }

    private ObservableCollection<ParamAqaraModel> _params;
    public ObservableCollection<ParamAqaraModel> Params
    {
        get => _params;
        set => SetProperty(ref _params, value);
    }

    private DeviceBindableModel _device;
    public DeviceBindableModel Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }
}

