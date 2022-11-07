using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Scenarios;
using SmartMirror.ViewModels.Tabs.Details;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class ScenariosPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IScenariosService _scenariosService;
    private readonly IDialogService _dialogService;

    private bool _isNeedReloadData = true;
    private bool _isPageFocused;

    public ScenariosPageViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IMapperService mapperService,
        IScenariosService scenariosService)
        : base(navigationService)
    {
        _dialogService = dialogService;
        _mapperService = mapperService;
        _scenariosService = scenariosService;

        Title = "Scenarios";

        DataState = EPageState.LoadingSkeleton;

        Task.Run(_scenariosService.DownloadAllScenariosAsync);

        _scenariosService.ScenariosChanged += OnScenariosChanged;
    }

    #region -- Public properties --

    private ObservableCollection<ScenarioBindableModel> _favoriteScenarios = new();
    public ObservableCollection<ScenarioBindableModel> FavoriteScenarios
    {
        get => _favoriteScenarios;
        set => SetProperty(ref _favoriteScenarios, value);
    }

    private ObservableCollection<ScenarioBindableModel> _scenarios = new();
    public ObservableCollection<ScenarioBindableModel> Scenarios
    {
        get => _scenarios;
        set => SetProperty(ref _scenarios, value);
    }

    private ICommand _runScenarioCommand;
    public ICommand RunScenarioCommand => _runScenarioCommand ??= SingleExecutionCommand.FromFunc<ScenarioBindableModel>(OnRunScenarioCommandAsync);

    private ICommand _goToScenarioDetailsCommand;
    public ICommand GoToScenarioDetailsCommand => _goToScenarioDetailsCommand ??= SingleExecutionCommand.FromFunc<ScenarioBindableModel>(OnGoToScenarioDetailsCommandAsync);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _scenariosService.ScenariosChanged -= OnScenariosChanged;

        base.Destroy();
    }

    public override void OnAppearing()
    {
        base.OnAppearing();

        if (_isNeedReloadData)
        {
            LoadScenariosAndChangeState();
        }
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();

        _isPageFocused = false;
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        base.OnNavigatedTo(parameters);

        _isNeedReloadData = true;
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.LoadingSkeleton;

                await ReloadScenariosAndChangeStateAsync();
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async void OnScenariosChanged(object sender, EventArgs e)
    {
        if (_scenariosService.AllScenarios is not null && _scenariosService.AllScenarios.Any())
        {
            if (_isPageFocused)
            {
                LoadScenariosAndChangeState();
            }
            else
            {
                _isNeedReloadData = true;
            }
        }
        else
        {
            DataState = EPageState.LoadingSkeleton;

            await ReloadScenariosAndChangeStateAsync();
        }
    }

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(ReloadScenariosAndChangeStateAsync, executionTime);
        }
    }

    private async Task<bool> ReloadScenariosAndChangeStateAsync()
    {
        var resultOfDownloadingScenarios = await _scenariosService.DownloadAllScenariosAsync();

        if (resultOfDownloadingScenarios.IsSuccess)
        {
            LoadScenariosAndChangeState();
        }
        else
        {
            Debug.WriteLine($"Can't download devices: {resultOfDownloadingScenarios.Message}");
        }

        return resultOfDownloadingScenarios.IsSuccess;
    }

    private void LoadScenariosAndChangeState()
    {
        var scenarios = _scenariosService.AllScenarios;

        SetScenariosCommands(scenarios);

        Scenarios = new(scenarios.Where(scenario => scenario.IsShownInScenarios));

        FavoriteScenarios = new(scenarios.Where(scenario => scenario.IsFavorite));

        DataState = scenarios.Any()
            ? EPageState.Complete
            : EPageState.Empty;
    }

    private void SetScenariosCommands(IEnumerable<ScenarioBindableModel> scenarios)
    {
        foreach (var scenario in scenarios)
        {
            scenario.ChangeActiveStatusCommand = RunScenarioCommand;
            scenario.TappedCommand = GoToScenarioDetailsCommand;
        }
    }

    private async Task OnRunScenarioCommandAsync(ScenarioBindableModel selectedScenario)
    {
        selectedScenario.IsUpdating = true;

        var resultOfUpdattingScenario = await _scenariosService.RunScenarioAsync(selectedScenario.SceneId);

        if (resultOfUpdattingScenario.IsSuccess)
        {
            UpdateStatusRunningScenario(FavoriteScenarios, selectedScenario.SceneId);
            UpdateStatusRunningScenario(Scenarios, selectedScenario.SceneId);
        }
        else
        {
            var errorDialogParameters = new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, "Error" },
                { Constants.DialogsParameterKeys.DESCRIPTION, resultOfUpdattingScenario.Message },
            };

            await _dialogService.ShowDialogAsync(nameof(ErrorDialog), errorDialogParameters);
        }

        selectedScenario.IsUpdating = false;
    }

    private Task OnGoToScenarioDetailsCommandAsync(ScenarioBindableModel scenario)
    {
        _isNeedReloadData = false;

        return NavigationService.CreateBuilder()
            .AddSegment<ScenarioDetailsPageViewModel>()
            .AddParameter(KnownNavigationParameters.Animated, true)
            .AddParameter(nameof(ScenarioBindableModel), scenario)
            .NavigateAsync();
    }

    private void UpdateStatusRunningScenario(IEnumerable<ScenarioBindableModel> scenarios, string scenarioId)
    {
        var scenario = scenarios?.FirstOrDefault(x => x.SceneId == scenarioId);

        if (scenario is not null)
        {
            scenario.IsActive = true;
        }
    }

    #endregion
}