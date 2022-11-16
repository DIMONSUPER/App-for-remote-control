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
using Plugin.Maui.Audio;

namespace SmartMirror.ViewModels.Dialogs
{
    public class DoorBellDialogViewModel : BaseDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IPermissionsService _permissionsService;
        private readonly ISettingsManager _settingsManager;
        private readonly ICamerasService _camerasService;
        private readonly IMapperService _mapperService;
        private readonly IAudioManager _audioManager;

        private IAudioPlayer _audioPlayer;

        public DoorBellDialogViewModel(
            IBlurService blurService,
            IDialogService dialogService,
            IPermissionsService permissionsService,
            ISettingsManager settingsManager,
            ICamerasService camerasService,
            IAudioManager audioManager,
            IMapperService mapperService)
            : base(blurService)
        {
            _dialogService = dialogService;
            _permissionsService = permissionsService;
            _settingsManager = settingsManager;
            _camerasService = camerasService;
            _mapperService = mapperService;
            _audioManager = audioManager;

            IsVideoOnTop = true;
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

        private bool _isVideoOnTop;
        public bool IsVideoOnTop
        {
            get => _isVideoOnTop;
            set => SetProperty(ref _isVideoOnTop, value);
        }

        private bool _isTalking;
        public bool IsTalking
        {
            get => _isTalking;
            set => SetProperty(ref _isTalking, value);
        }

        private DeviceBindableModel _device;
        public DeviceBindableModel Device
        {
            get => _device;
            set => SetProperty(ref _device, value);
        }

        private ICommand _onTalkCommand;
        public ICommand OnTalkCommand => _onTalkCommand ??= SingleExecutionCommand.FromFunc(OnTalkCommandAsync);

        private ICommand _onEndCommand;
        public ICommand OnEndCommand => _onEndCommand ??= SingleExecutionCommand.FromFunc(OnEndCommandAsync);

        private ICommand _videoPaybackErrorCommand;
        public ICommand VideoPaybackErrorCommand => _videoPaybackErrorCommand ??= SingleExecutionCommand.FromFunc(OnVideoPaybackErrorCommandAsync);

        private ICommand _tryAgainCommand;
        public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

        #endregion

        #region -- Overrides --

        public async override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.ACCESSORY, out DeviceBindableModel device))
            {
                Device = device;

                if (device.Status == EDeviceStatus.On)
                {
                    var fileStream = await FileSystem.OpenAppPackageFileAsync(Constants.Rings.DOORBELL);

                    _audioPlayer = _audioManager.CreatePlayer(fileStream);

                    _audioPlayer.Volume = 1;

                    _audioPlayer.Play();
                }
            }

            if (IsInternetConnected)
            {
                DataState = EPageState.Loading;

                var isDataLoaded = await LoadCameraAsync();

                if (VideoSource is not null)
                {
                    (DataState, VideoAction) = isDataLoaded
                        ? (EPageState.Complete, EVideoAction.Play)
                        : (EPageState.Loading, EVideoAction.Pause);
                }
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();

            _audioPlayer?.Dispose();

            IsVideoOnTop = false;
        }

        #endregion

        #region -- Private helpers --

        private async Task OnTalkCommandAsync()
        {
            if (IsTalking)
            {
                IsTalking = false;
            }
            else
            {
                var status = await _permissionsService.RequestPermissionAsync<Permissions.Microphone>();

                if (status != PermissionStatus.Granted)
                {
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
                else
                {
                    IsTalking = true;
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

        private async Task OnTryAgainCommandAsync()
        {
            DataState = EPageState.NoInternetLoader;

            var isDataLoaded = await LoadCameraAsync();

            if (IsInternetConnected)
            {
                (DataState, VideoAction) = isDataLoaded
                    ? (EPageState.Complete, EVideoAction.Play)
                    : (EPageState.Loading, EVideoAction.Pause);
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }

        private async Task<bool> LoadCameraAsync()
        {
            var result = false;

            await Task.Delay(1000);

            VideoSource = "https://videos-3.earthcam.com/fecnetwork/15659.flv/chunklist_w999153032.m3u8";

            return result;
        }

        #endregion
    }
}

