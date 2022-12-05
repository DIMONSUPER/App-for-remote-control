using System.Windows.Input;
using SmartMirror.Enums;
using SmartMirror.Interfaces;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Models.BindableModels;

public class AutomationBindableModel : BindableBase, ITappable
{
    #region -- Public properties --

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _linkageId;
    public string LinkageId
    {
        get => _linkageId;
        set => SetProperty(ref _linkageId, value);
    }

    private string _positionId;
    public string PositionId
    {
        get => _positionId;
        set => SetProperty(ref _positionId, value);
    }

    private string _model;
    public string Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

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

    private ELocalizeAqara _localize;
    public ELocalizeAqara Localize
    {
        get => _localize;
        set => SetProperty(ref _localize, value);
    }

    private LinkageConditionsAqaraModel _conditions;
    public LinkageConditionsAqaraModel Conditions
    {
        get => _conditions;
        set => SetProperty(ref _conditions, value);
    }

    private LinkageActionsAqaraModel _actions;
    public LinkageActionsAqaraModel Actions
    {
        get => _actions;
        set => SetProperty(ref _actions, value);
    }

    private bool _enable;
    public bool Enable
    {
        get => _enable;
        set => SetProperty(ref _enable, value);
    }

    private bool _isExecuting;
    public bool IsExecuting
    {
        get => _isExecuting;
        set => SetProperty(ref _isExecuting, value);
    }

    private bool _isShownInAutomations = true;
    public bool IsShownInAutomations
    {
        get => _isShownInAutomations;
        set => SetProperty(ref _isShownInAutomations, value);
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

    private bool _isEmergencyNotification = true;
    public bool IsEmergencyNotification
    {
        get => _isEmergencyNotification;
        set => SetProperty(ref _isEmergencyNotification, value);
    }

    private ICommand _changeActiveStatusCommand;
    public ICommand ChangeActiveStatusCommand
    {
        get => _changeActiveStatusCommand;
        set => SetProperty(ref _changeActiveStatusCommand, value);
    }

    #endregion

    #region -- ITappable implementation --

    private ICommand _tapCommand;
    public ICommand TapCommand
    {
        get => _tapCommand;
        set => SetProperty(ref _tapCommand, value);
    }

    #endregion
}

