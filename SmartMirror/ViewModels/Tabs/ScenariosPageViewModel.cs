using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Scenarios;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class ScenariosPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IScenariosService _scenariosService;
    private readonly IDialogService _dialogService;

    private bool _isNeedReloadData = true;

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

    public override async void OnAppearing()
    {
        base.OnAppearing();

        if (_isNeedReloadData && !IsDataLoading)
        {
            DataState = EPageState.Loading;

            await LoadScenariosAsyncAndChangeState();
        }
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
                DataState = EPageState.Loading;

                await LoadScenariosAsyncAndChangeState();
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadScenariosAsync, executionTime);

            if (IsInternetConnected)
            {
                DataState = isDataLoaded
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
    }

    private async Task OnRunScenarioCommandAsync(ScenarioBindableModel selectedScenario)
    {
        selectedScenario.IsUpdating = true;

        var resultOfUpdattingScenario = await _scenariosService.RunScenarioAsync(selectedScenario.Id);

        if (resultOfUpdattingScenario.IsSuccess)
        {
            UpdateStatusRunningScenario(FavoriteScenarios, selectedScenario.Id);
            UpdateStatusRunningScenario(Scenarios, selectedScenario.Id);
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

    private async Task LoadScenariosAsyncAndChangeState()
    {
        if (IsInternetConnected)
        {
            var isDataLoaded = await LoadScenariosAsync();

            if (IsInternetConnected)
            {
                DataState = isDataLoaded
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
        else
        {     
            DataState = EPageState.NoInternet;
        }
    }

    private async Task<bool> LoadScenariosAsync()
    {
        bool isLoaded = false;

        if (IsInternetConnected)
        {
            var loadingScenariosResults = await Task.WhenAll(
                LoadFavoritesScenariosAsync(),
                LoadAllScenariosAsync());

            isLoaded = loadingScenariosResults.Any(x => x);
        }

        return isLoaded;
    }

    private async Task<bool> LoadFavoritesScenariosAsync()
    {
        var resultOfGettingFavoriteScenarios = await _scenariosService.GetFavoriteScenariosAsync();

        if (resultOfGettingFavoriteScenarios.IsSuccess)
        {
            var favoriteScenarios = GetBindableModelWithSetCommands(resultOfGettingFavoriteScenarios.Result);

            FavoriteScenarios = new(favoriteScenarios);
        }

        return resultOfGettingFavoriteScenarios.IsSuccess;
    }

    private async Task<bool> LoadAllScenariosAsync()
    {
        var resultOfGettingAllScenarios = await _scenariosService.GetScenariosAsync();

        if (resultOfGettingAllScenarios.IsSuccess)
        {
            var allScenarios = GetBindableModelWithSetCommands(resultOfGettingAllScenarios.Result);

            Scenarios = new(allScenarios);
        }

        return resultOfGettingAllScenarios.IsSuccess;
    }

    private IEnumerable<ScenarioBindableModel> GetBindableModelWithSetCommands(IEnumerable<ScenarioModel> scenarios)
    {
        return _mapperService.MapRange<ScenarioBindableModel>(scenarios, (m, vm) =>
        {
            vm.ChangeActiveStatusCommand = RunScenarioCommand;
            vm.TappedCommand = GoToScenarioDetailsCommand;
        });
    }

    private void UpdateStatusRunningScenario(IEnumerable<ScenarioBindableModel> scenarios, string scenarioId)
    {
        var scenario = scenarios?.FirstOrDefault(x => x.Id == scenarioId);

        if (scenario is not null)
        {
            scenario.IsActive = true;
        }
    }

    #endregion
}