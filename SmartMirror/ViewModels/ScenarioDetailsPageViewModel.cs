using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class ScenarioDetailsPageViewModel : BaseViewModel
    {
        public ScenarioDetailsPageViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
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

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(ScenarioBindableModel), out ScenarioBindableModel scenario))
            {
                ScenarioName = scenario.Name;

                DataState = EPageState.Loading;

                await Task.Delay(2000);

                ScenarioActions = new(scenario.ScenarioActions);

                DataState = ScenarioActions.Count > 0
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
        }

        protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                DataState = EPageState.Loading;

                await Task.Delay(2000);

                DataState = ScenarioActions.Count > 0
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
