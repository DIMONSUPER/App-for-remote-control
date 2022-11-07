using System;
using System.ComponentModel;
using System.Windows.Input;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Permissions;
using SmartMirror.Services.Settings;
using SmartMirror.Views.Dialogs;
using SmartMirror.Resources.Strings;
using CommunityToolkit.Maui.Alerts;

namespace SmartMirror.ViewModels.Dialogs
{
    public class DoorBellDialogViewModel : BaseDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IPermissionsService _permissionsService;
        private readonly ISettingsManager _settingsManager;
        private readonly ICamerasService _camerasService;
        private readonly IMapperService _mapperService;

        public DoorBellDialogViewModel(
            IBlurService blurService,
            IDialogService dialogService,
            IPermissionsService permissionsService,
            ISettingsManager settingsManager,
            ICamerasService camerasService,
            IMapperService mapperService)
            : base(blurService)
        {
            _dialogService = dialogService;
            _permissionsService = permissionsService;
            _settingsManager = settingsManager;
            _camerasService = camerasService;
            _mapperService = mapperService;
        }

        #region -- Public properties --

        private EPageState _dataState;
        public EPageState DataState
        {
            get => _dataState;
            set => SetProperty(ref _dataState, value);
        }

        private EVideoAction _videoAction;
        public EVideoAction VideoAction
        {
            get => _videoAction;
            set => SetProperty(ref _videoAction, value);
        }

        private string _videoSource;
        public string VideoSource
        {
            get => _videoSource;
            set => SetProperty(ref _videoSource, value);
        }

        private double _videoPlayerVolume = 100;
        public double VideoPlayerVolume 
        {
            get => _videoPlayerVolume;
            set => SetProperty(ref _videoPlayerVolume, value);
        }

        private ICommand _onTalkCommand;
        public ICommand OnTalkCommand => _onTalkCommand ??= SingleExecutionCommand.FromFunc(OnTalkCommandAsync);

        private ICommand _onEndCommand;
        public ICommand OnEndCommand => _onEndCommand ??= SingleExecutionCommand.FromFunc(OnEndCommandAsync);

        private ICommand _videoPaybackErrorCommand;
        public ICommand VideoPaybackErrorCommand => _videoPaybackErrorCommand ??= SingleExecutionCommand.FromFunc(OnVideoPaybackErrorCommandAsync);

        #endregion

        #region -- Overrides --

        public async override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            DataState = EPageState.Loading;

            var isDataLoaded = await LoadCameraAsync();

            if (VideoSource is not null)
            {
                (DataState, VideoAction) = isDataLoaded
                    ? (EPageState.Complete, EVideoAction.Play)
                    : (EPageState.Loading, EVideoAction.Pause);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnTalkCommandAsync()
        {
            var status = await _permissionsService.RequestPermissionAsync<Permissions.Microphone>();

            if (status == PermissionStatus.Granted)
            {
                return;
            }

            var rationalePermission = _permissionsService.ShouldShowRationale<Permissions.Microphone>();

            if (!rationalePermission)
            {
                var dialogResult = await _dialogService.ShowDialogAsync(nameof(ConfirmDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Permission alert" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, "You need give permissions for microphone, open application settings?" }
                });

                if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result) && result)
                {
                    _permissionsService.OpenApplicationSettingsPage();
                }
            }
        }

        private Task OnEndCommandAsync()
        {
            RequestClose.Invoke();

            return Task.CompletedTask;
        }

        private Task OnVideoPaybackErrorCommandAsync()
        {
            Toast.Make(Strings.CannotPlayVideo).Show();

            return Task.CompletedTask;
        }

        private async Task<bool> LoadCameraAsync()
        {
            var result = false;

            var resultOfGettingCameras = await _camerasService.GetCamerasAsync();

            await Task.Delay(1000);

            if (resultOfGettingCameras.IsSuccess)
            {
                var camera = resultOfGettingCameras.Result.FirstOrDefault();
                VideoSource = camera.VideoUrl;

                result = resultOfGettingCameras.IsSuccess;
            }

            return result;
        }

        #endregion
    }
}

