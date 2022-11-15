using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Scenarios;
using SmartMirror.ViewModels.Tabs.Details;
using SmartMirror.Views.Dialogs;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class ScenariosPageViewModel : BaseTabViewModel
{
    private readonly IMapperService _mapperService;
    private readonly IScenariosService _scenariosService;
    private readonly IDialogService _dialogService;

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

        _scenariosService.AllScenariosChanged += OnScenariosChanged;
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

    #endregion

    #region -- Overrides --

    public override void Destroy()
    {
        _scenariosService.AllScenariosChanged -= OnScenariosChanged;

        base.Destroy();
    }

    #endregion

    #region -- Private helpers --

    private async void OnScenariosChanged(object sender, EventArgs e)
    {
        await LoadScenariosAndChangeStateAsync();
    }

    private async Task LoadScenariosAndChangeStateAsync()
    {
        var scenarios = await _scenariosService.GetAllScenariosAsync();

        if (scenarios.Any())
        {
            SetScenariosCommands(scenarios);

            Scenarios = new(scenarios.Where(scenario => scenario.IsShownInScenarios));

            FavoriteScenarios = new(scenarios.Where(scenario => scenario.IsFavorite));

            DataState = (Scenarios.Any() || FavoriteScenarios.Any())
                ? EPageState.Complete
                : EPageState.Empty;
        }
        else
        {
            DataState = EPageState.Empty;
        }
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
        IsNeedReloadData = false;

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