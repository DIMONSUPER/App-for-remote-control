using System.Windows.Input;
using SmartMirror.Models.BindableModels;
using SmartMirror.Helpers;
using SmartMirror.Enums;
using SmartMirror.Services.Mapper;

namespace SmartMirror.ViewModels.Tabs.Details;

public class AutomationDetailsPageViewModel : BaseViewModel
{
    private readonly IMapperService _mapperService;

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

    private bool _relation;
    public bool Relation
    {
        get => _relation;
        set => SetProperty(ref _relation, value);
    }

    private List<AutomationDetailCardBindableModel> _automationConditions;
    public List<AutomationDetailCardBindableModel> AutomationConditions
    {
        get => _automationConditions;
        set => SetProperty(ref _automationConditions, value);
    }

    private List<AutomationDetailCardBindableModel> _automationActions;
    public List<AutomationDetailCardBindableModel> AutomationActions
    {
        get => _automationActions;
        set => SetProperty(ref _automationActions, value);
    }

    private AutomationBindableModel _automationBindableModel;
    public AutomationBindableModel AutomationBindableModel
    {
        get => _automationBindableModel;
        set => SetProperty(ref _automationBindableModel, value);
    }

    private ICommand _goBackCommand;
    public ICommand GoBackCommand => _goBackCommand ??= SingleExecutionCommand.FromFunc(OnGoBackCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    #endregion

    #region -- Overrides --

    public override void Initialize(INavigationParameters parameters)
    {
        base.Initialize(parameters);

        if (parameters.TryGetValue(nameof(AutomationBindableModel), out AutomationBindableModel automation))
        {
            DataState = EPageState.LoadingSkeleton;

            AutomationBindableModel = automation;

            Relation = automation.Relation;

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
        var automationActions = _mapperService.MapRange<ActionBindableModel, AutomationDetailCardBindableModel>(_automationBindableModel.Actions, (m, vm) =>
        {
            vm.TriggerName = m.ActionName;
        });

        AutomationActions = new(automationActions);

        var automationConditions = _mapperService.MapRange<AutomationDetailCardBindableModel>(_automationBindableModel.Conditions);

        AutomationConditions = new(automationConditions);

        DataState = AutomationActions?.Count > 0 && AutomationConditions?.Count > 0
                ? EPageState.Complete
                : EPageState.Empty;

        return Task.CompletedTask;
    }

    private Task OnGoBackCommandAsync()
    {
        return NavigationService.GoBackAsync();
    }

    #endregion
}

