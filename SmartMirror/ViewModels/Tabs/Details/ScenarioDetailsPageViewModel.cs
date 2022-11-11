using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Rooms;
using SmartMirror.Services.Scenarios;
using SmartMirror.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs.Details
{
    public class ScenarioDetailsPageViewModel : BaseViewModel
    {
        private readonly IScenariosService _scenariosService;
        private readonly IRoomsService _roomsService;
        private readonly IDialogService _dialogService;

        private ScenarioBindableModel _scenarioBindableModel;

        public ScenarioDetailsPageViewModel(
            INavigationService navigationService,
            IScenariosService scenariosService,
            IRoomsService roomsService,
            IDialogService dialogService) 
            : base(navigationService)
        {
            _roomsService = roomsService;
            _scenariosService = scenariosService;
            _dialogService = dialogService;
        }

        #region -- Public properties --

        private string _scenarioName;
        public string ScenarioName
        {
            get => _scenarioName;
            set => SetProperty(ref _scenarioName, value);
        }

        private ObservableCollection<ScenarioActionBindableModel> _scenarioActions;
        public ObservableCollection<ScenarioActionBindableModel> ScenarioActions
        {
            get => _scenarioActions;
            set => SetProperty(ref _scenarioActions, value);
        }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= SingleExecutionCommand.FromFunc(OnGoBackCommandAsync);

        private ICommand _tryAgainCommand;
        public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(ScenarioBindableModel), out ScenarioBindableModel scenario))
            {
                DataState = EPageState.LoadingSkeleton;

                _scenarioBindableModel = scenario;

                Task.Run(async () =>
                {
                    await LoadScenarioInformationAsync(_scenarioBindableModel);

                    DataState = ScenarioActions?.Count > 0
                        ? EPageState.Complete
                        : EPageState.Empty;
                });
            }
        }

        protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                DataState = EPageState.LoadingSkeleton;

                await LoadScenarioInformationAsync(_scenarioBindableModel);

                DataState = ScenarioActions?.Count > 0
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task LoadScenarioInformationAsync(ScenarioBindableModel scenario)
        {
            var scenarioDetailInformation = await _scenariosService.GetScenarioByIdAsync(scenario.SceneId);

            if (scenarioDetailInformation.IsSuccess)
            {
                var rooms = await _roomsService.GetAllRoomsAsync();

                ScenarioName = scenarioDetailInformation.Result.Name;
                ScenarioActions = scenarioDetailInformation.Result.Actions;
           
                foreach (var action in ScenarioActions)
                {
                    if (action.Device is not null)
                    {
                        action.Device.RoomName = rooms.FirstOrDefault(x => x.Id == action.Device.PositionId)?.Name;
                    }
                }
            }
            else
            {
                var errorDialogParameters = new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Error" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, scenarioDetailInformation.Message },
                };

                MainThread.BeginInvokeOnMainThread(async () => await _dialogService.ShowDialogAsync(nameof(ErrorDialog), errorDialogParameters));
            }
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
}
