using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Automation;
using SmartMirror.Services.Mapper;
using SmartMirror.ViewModels.Tabs.Details;
using SmartMirror.Views.Dialogs;
using SmartMirror.Resources.Strings;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class AutomationPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IAutomationService _automationService;
    private readonly IDialogService _dialogService;

    public AutomationPageViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IMapperService mapperService,
        IAutomationService automationService)
        : base(navigationService)
    {
        _dialogService = dialogService;
        _mapperService = mapperService;
        _automationService = automationService;

        Title = "Automation";

        DataState = EPageState.LoadingSkeleton;

        _automationService.AllAutomationsChanged += OnAutomationsChanged;
    }

    #region -- Public properties --

    private ObservableCollection<AutomationBindableModel> _favoriteAutomations = new();
    public ObservableCollection<AutomationBindableModel> FavoriteAutomations
    {
        get => _favoriteAutomations;
        set => SetProperty(ref _favoriteAutomations, value);
    }

    private ObservableCollection<AutomationBindableModel> _automations = new();
    public ObservableCollection<AutomationBindableModel> Automations
    {
        get => _automations;
        set => SetProperty(ref _automations, value);
    }

    private ICommand _runAutomationCommand;
    public ICommand RunAutomationCommand => _runAutomationCommand ??= SingleExecutionCommand.FromFunc<AutomationBindableModel>(OnRunAutomationCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    private ICommand _goToAutomationDetailsCommand;
    public ICommand GoToAutomationDetailsCommand => _goToAutomationDetailsCommand ??= SingleExecutionCommand.FromFunc<AutomationBindableModel>(OnGoToAutomationDetailsCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _automationService.AllAutomationsChanged -= OnAutomationsChanged;

        base.Destroy();
    }

    #endregion

    #region -- Private helpers --

    private async void OnAutomationsChanged(object sender, EventArgs e)
    {
        await LoadAutomationsAndChangeStateAsync();
    }

    private async Task LoadAutomationsAndChangeStateAsync()
    {
        var automations = await _automationService.GetAllAutomationsAsync();

        if (automations.Any())
        {
            SetAutomationsCommands(automations);

            Automations = new(automations.Where(automation => automation.IsShownInAutomations));

            FavoriteAutomations = new(automations.Where(scenario => scenario.IsFavorite));

            DataState = (Automations.Any() || FavoriteAutomations.Any())
                ? EPageState.Complete
                : EPageState.Empty;
        }
        else
        {
            DataState = EPageState.Empty;
        }
    }

    private void SetAutomationsCommands(IEnumerable<AutomationBindableModel> automations)
    {
        foreach (var automation in automations)
        {
            automation.ChangeActiveStatusCommand = RunAutomationCommand;
            automation.TapCommand = GoToAutomationDetailsCommand;
        }
    }

    private async Task OnRunAutomationCommandAsync(AutomationBindableModel selectedAutomation)
    {
        selectedAutomation.IsExecuting = true;

        var changeResponse = await _automationService.ChangeLinkageStateAsync(selectedAutomation.LinkageId, !selectedAutomation.Enable);

        selectedAutomation.IsExecuting = false;

        if (changeResponse.IsSuccess)
        {
            selectedAutomation.Enable = !selectedAutomation.Enable;
        }
        else
        {
            var errorDescription = IsInternetConnected
                ? changeResponse.Message
                : Strings.NoInternetConnection;

            await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                { Constants.DialogsParameterKeys.DESCRIPTION, errorDescription },
            });
        }
    }

    private Task OnGoToAutomationDetailsCommandAsync(AutomationBindableModel automation)
    {
        IsNeedReloadData = false;

        return NavigationService.CreateBuilder()
            .AddSegment<AutomationDetailsPageViewModel>()
            .AddParameter(KnownNavigationParameters.Animated, true)
            .AddParameter(nameof(AutomationBindableModel), automation)
            .NavigateAsync();
    }

    #endregion
}