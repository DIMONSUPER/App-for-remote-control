using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SmartMirror.Models.BindableModels;
using SmartMirror.Helpers;
using SmartMirror.Enums;
using SmartMirror.Models.Aqara;
using SmartMirror.Services.Mapper;

namespace SmartMirror.ViewModels.Tabs.Details;

public class AutomationDetailsPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;

    private AutomationBindableModel _automationBindableModel;

    public AutomationDetailsPageViewModel(
        IMapperService mapperService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _mapperService = mapperService;
    }

    #region -- Public properties --

    private string _automationName;
    public string AutomationName
    {
        get => _automationName;
        set => SetProperty(ref _automationName, value);
    }

    private ObservableCollection<AutomationDetailCardBindableModel> _automationActions;
    public ObservableCollection<AutomationDetailCardBindableModel> AutomationActions
    {
        get => _automationActions;
        set => SetProperty(ref _automationActions, value);
    }

    private ICommand _goBackCommand;
    public ICommand GoBackCommand => _goBackCommand ??= SingleExecutionCommand.FromFunc(OnGoBackCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public override void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        if (parameters.TryGetValue(nameof(AutomationBindableModel), out AutomationBindableModel automation))
        {
            DataState = EPageState.LoadingSkeleton;

            _automationBindableModel = automation;

            LoadAutomationInformationAsync();
        }
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            DataState = EPageState.LoadingSkeleton;

            await LoadAutomationInformationAsync();
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private Task LoadAutomationInformationAsync()
    {
        var automationActions = _mapperService.MapRange<AutomationDetailCardBindableModel>(_automationBindableModel.Actions.Action);
        var automationConditions = _mapperService.MapRange<AutomationDetailCardBindableModel>(_automationBindableModel.Conditions.Condition);

        AutomationActions = new(automationConditions.Concat(automationActions));

        DataState = AutomationActions?.Count > 0
                ? EPageState.Complete
                : EPageState.Empty;

        return Task.CompletedTask;
    }

    private Task OnTryAgainCommandAsync()
    {
        return Task.CompletedTask;
    }

    private Task OnGoBackCommandAsync()
    {
        return NavigationService.GoBackAsync();
    }

    #endregion
}

