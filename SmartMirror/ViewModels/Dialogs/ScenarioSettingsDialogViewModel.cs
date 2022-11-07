using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Scenarios;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ScenarioSettingsDialogViewModel : BaseDialogViewModel
    {
        private readonly IScenariosService _scenariosService;

        public ScenarioSettingsDialogViewModel(
            IScenariosService scenariosService,
            IBlurService blurService)
            : base(blurService)
        {
            _scenariosService = scenariosService;
        }

        #region -- Public properties --

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isShownInScenarios;
        public bool IsShownInScenarios
        {
            get => _isShownInScenarios;
            set => SetProperty(ref _isShownInScenarios, value);
        }

        private bool _isReceiveNotifications;
        public bool IsReceiveNotifications
        {
            get => _isReceiveNotifications;
            set => SetProperty(ref _isReceiveNotifications, value);
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        private ScenarioBindableModel _scenario;
        public ScenarioBindableModel Scenario
        {
            get => _scenario;
            set => SetProperty(ref _scenario, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogsParameterKeys.SCENARIO, out ImageAndTitleBindableModel scenario))
            {
                Title = scenario.Name;

                Scenario = scenario.Model as ScenarioBindableModel;

                IsShownInScenarios = Scenario.IsShownInScenarios;
                IsReceiveNotifications = Scenario.IsReceiveNotifications;
                IsFavorite = Scenario.IsFavorite;
            }
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(IsFavorite) or nameof(IsShownInScenarios) or nameof(IsReceiveNotifications))
            {
                Scenario.IsFavorite = _isFavorite;
                Scenario.IsShownInScenarios = _isShownInScenarios;
                Scenario.IsReceiveNotifications = _isReceiveNotifications;

                await _scenariosService.UpdateScenarioAsync(Scenario);
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnCloseCommandAsync()
        {
            RequestClose.Invoke();

            return Task.CompletedTask;
        }

        #endregion
    }
}
