﻿using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Scenarios;
using SmartMirror.Services.Keyboard;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class ScenarioSettingsDialogViewModel : BaseDialogViewModel
    {
        private readonly IScenariosService _scenariosService;
        private bool _isInitializing = true;

        public ScenarioSettingsDialogViewModel(
            IScenariosService scenariosService,
            IBlurService blurService,
            IKeyboardService keyboardService)
            : base(blurService, keyboardService)
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

        private bool _isEmergencyNotification;
        public bool IsEmergencyNotification
        {
            get => _isEmergencyNotification;
            set => SetProperty(ref _isEmergencyNotification, value);
        }

        private ScenarioBindableModel _scenario;
        public ScenarioBindableModel Scenario
        {
            get => _scenario;
            set => SetProperty(ref _scenario, value);
        }

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.SCENARIO, out ImageAndTitleBindableModel scenario))
            {
                Title = scenario.Name;

                Scenario = scenario.Model as ScenarioBindableModel;

                IsShownInScenarios = Scenario.IsShownInScenarios;
                IsReceiveNotifications = Scenario.IsReceiveNotifications;
                IsFavorite = Scenario.IsFavorite;
                IsEmergencyNotification = Scenario.IsEmergencyNotification;
            }

            _isInitializing = false;
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (!_isInitializing
                && args.PropertyName is nameof(IsFavorite)
                or nameof(IsShownInScenarios)
                or nameof(IsReceiveNotifications)
                or nameof(IsEmergencyNotification))
            {
                Scenario.IsFavorite = _isFavorite;
                Scenario.IsShownInScenarios = _isShownInScenarios;
                Scenario.IsReceiveNotifications = _isReceiveNotifications;
                Scenario.IsEmergencyNotification = _isEmergencyNotification;

                await _scenariosService.UpdateScenarioAsync(Scenario);
            }
        }

        #endregion
    }
}
