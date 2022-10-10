using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Mapper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class ScenarioPageViewModel : BaseViewModel
    {
        private readonly IMapperService _mapperService;

        public ScenarioPageViewModel(
            INavigationService navigationService,
            IMapperService mapperService) 
            : base(navigationService)
        {
            _mapperService = mapperService;
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

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(nameof(ScenarioBindableModel), out ScenarioBindableModel scenario))
            {
                ScenarioName = scenario.Name;
                ScenarioActions = new(scenario.ScenarioActions);
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnGoBackCommandAsync()
        {
            return NavigationService.GoBackAsync();
        }

        #endregion
    }
}
