using System;
using System.Collections.ObjectModel;

namespace SmartMirror.Models.BindableModels;

public class AutomationDetailCardBindableModel : BindableBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _triggerName;
    public string TriggerName
    {
        get => _triggerName;
        set => SetProperty(ref _triggerName, value);
    }

    private ObservableCollection<ConditionBindableModel> _conditions;
    public ObservableCollection<ConditionBindableModel> Conditions
    {
        get => _conditions;
        set => SetProperty(ref _conditions, value);
    }

    private DeviceBindableModel _device;
    public DeviceBindableModel Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }
}