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
        var resultOfGettingFavoriteScenarios = await _scenariosService.GetFavoriteScenariosAsync();

        if (resultOfGettingFavoriteScenarios.IsSuccess)
        {
            var favoriteScenarios = await GetBindableModelWithSetCommandsAsync(resultOfGettingFavoriteScenarios.Result);

            FavoriteScenarios = new(favoriteScenarios);
        }

        var resultOfGettingAllScenarios = await _scenariosService.GetAllScenariosAsync();

        if (resultOfGettingAllScenarios.IsSuccess)
        {
            var allScenarios = await GetBindableModelWithSetCommandsAsync(resultOfGettingAllScenarios.Result);

            Scenarios = new(allScenarios);
        }

        DataState = EPageState.Complete;
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

