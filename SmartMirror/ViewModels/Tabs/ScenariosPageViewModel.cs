using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Scenarios;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs;

public class ScenariosPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IScenariosService _scenariosService;

    public ScenariosPageViewModel(
        IMapperService mapperService,
        IScenariosService scenariosService,
        INavigationService navigationService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _scenariosService = scenariosService;
        
        Title = "Scenarios";
        DataState = EPageState.Loading;

        ConnectivityChanged += OnConnectivityChanged;
    }

    #region -- Public properties --

    private ObservableCollection<ScenarioBindableModel> _favoriteScenarios;
    public ObservableCollection<ScenarioBindableModel> FavoriteScenarios
    {
        get => _favoriteScenarios;
        set => SetProperty(ref _favoriteScenarios, value);
    }

    private ObservableCollection<ScenarioBindableModel> _scenarios;
    public ObservableCollection<ScenarioBindableModel> Scenarios
    {
        get => _scenarios;
        set => SetProperty(ref _scenarios, value);
    }

    private ICommand _changeActiveStatusCommand;
    public ICommand ChangeActiveStatusCommand => _changeActiveStatusCommand ??= SingleExecutionCommand.FromFunc<ScenarioBindableModel>(OnChangeActiveStatusCommandAsync);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public override async Task InitializeAsync(INavigationParameters parameters)
    {
        await base.InitializeAsync(parameters);

        await LoadScenariosAsync();
    }

    #endregion

    #region -- Private helpers --

    private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await LoadScenariosAsync();
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private async Task OnTryAgainCommandAsync()
    {
        DataState = EPageState.NoInternetLoader;

        await Task.Delay(1000);

        await LoadScenariosAsync();
    }

    private async Task OnChangeActiveStatusCommandAsync(ScenarioBindableModel scenario)
    {
        var resultOfUpdattingScenario = await _scenariosService.UpdateActiveStatusScenarioAsync(scenario.Id, !scenario.IsActive);

        if (resultOfUpdattingScenario.IsSuccess)
        {
            scenario.IsActive = !scenario.IsActive;
        }
    }

    private async Task LoadScenariosAsync()
    {
        if (IsInternetConnected)
        {
            List<Task<bool>> tasks = new();

            tasks.Add(Task.Run(() => LoadFavoritesScenariosAsync()));
            tasks.Add(Task.Run(() => LoadAllScenariosAsync()));

            var tasksResult = await Task.WhenAll(tasks);

            var isSuccessLoadScenarios = !tasksResult.Any(row => row == false);

            if (isSuccessLoadScenarios)
            {
                DataState = EPageState.Complete;
            }
            else if(!IsInternetConnected)
            {
                DataState = EPageState.NoInternet;
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private async Task<bool> LoadFavoritesScenariosAsync()
    {
        var resultOfGettingFavoriteScenarios = await _scenariosService.GetFavoriteScenariosAsync();

        if (resultOfGettingFavoriteScenarios.IsSuccess)
        {
            var favoriteScenarios = await GetBindableModelWithSetCommandsAsync(resultOfGettingFavoriteScenarios.Result);

            FavoriteScenarios = new(favoriteScenarios);
        }

        return resultOfGettingFavoriteScenarios.IsSuccess;
    }

    private async Task<bool> LoadAllScenariosAsync()
    {
        var resultOfGettingAllScenarios = await _scenariosService.GetAllScenariosAsync();

        if (resultOfGettingAllScenarios.IsSuccess)
        {
            var allScenarios = await GetBindableModelWithSetCommandsAsync(resultOfGettingAllScenarios.Result);

            Scenarios = new(allScenarios);
        }

        return resultOfGettingAllScenarios.IsSuccess;
    }

    private Task<IEnumerable<ScenarioBindableModel>> GetBindableModelWithSetCommandsAsync(IEnumerable<ScenarioModel> scenarios)
    {
        return _mapperService.MapRangeAsync<ScenarioBindableModel>(scenarios, (m, vm) =>
        {
            vm.ChangeActiveStatusCommand = ChangeActiveStatusCommand;
        });
    }

    #endregion
}

