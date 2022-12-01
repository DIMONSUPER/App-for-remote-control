using System;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Services.Scenarios;
using SmartMirror.Services.Mapper;
using SmartMirror.Models.BindableModels;
using System.ComponentModel;
using SmartMirror.Models.DTO;
using SmartMirror.Services.Automation;

namespace SmartMirror.ViewModels.Dialogs;

public class AutomationSettingsDialogViewModel : BaseDialogViewModel
{
    private readonly IAutomationService _automationService;
    private readonly IMapperService _mapperService;
    private bool _isInitializing = true;

    public AutomationSettingsDialogViewModel(
        IAutomationService automationService,
        IMapperService mapperService,
        IBlurService blurService,
        IKeyboardService keyboardService)
        : base(blurService, keyboardService)
    {
        _automationService = automationService;
        _mapperService = mapperService;
    }

    #region -- Public properties --

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private bool _isShownInAutomations;
    public bool IsShownInAutomations
    {
        get => _isShownInAutomations;
        set => SetProperty(ref _isShownInAutomations, value);
    }

    private bool _isReceiveNotifications;
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

    private bool _isEmergencyNotification;
    public bool IsEmergencyNotification
    {
        get => _isEmergencyNotification;
        set => SetProperty(ref _isEmergencyNotification, value);
    }

    private AutomationBindableModel _automation;
    public AutomationBindableModel Automation
    {
        get => _automation;
        set => SetProperty(ref _automation, value);
    }

    #endregion

    #region -- Overrides --

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        base.OnDialogOpened(parameters);

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.AUTOMATION, out ImageAndTitleBindableModel automation))
        {
            Title = automation.Name;
            
            Automation = automation.Model as AutomationBindableModel;

            IsShownInAutomations = Automation.IsShownInAutomations;
            IsReceiveNotifications = Automation.IsReceiveNotifications;
            IsFavorite = Automation.IsFavorite;
            IsEmergencyNotification = Automation.IsEmergencyNotification;
        }

        _isInitializing = false;
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (!_isInitializing
            && args.PropertyName is nameof(IsFavorite)
            or nameof(IsShownInAutomations)
            or nameof(IsReceiveNotifications)
            or nameof(IsEmergencyNotification))
        {
            Automation.IsFavorite = args.PropertyName is nameof(IsFavorite) ? _isFavorite : Automation.IsFavorite;

            Automation.IsShownInAutomations = args.PropertyName is nameof(IsShownInAutomations) ? _isShownInAutomations : Automation.IsShownInAutomations;

            Automation.IsReceiveNotifications = args.PropertyName is nameof(IsReceiveNotifications) ? _isReceiveNotifications : Automation.IsReceiveNotifications;

            Automation.IsEmergencyNotification = args.PropertyName is nameof(IsEmergencyNotification) ? _isEmergencyNotification : Automation.IsEmergencyNotification;

            await _automationService.UpdateAutomationAsync(Automation);
        }
    }

    #endregion
}

