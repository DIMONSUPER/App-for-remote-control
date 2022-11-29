using CommunityToolkit.Maui.Alerts;
using LibVLCSharp.Shared;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class CamerasPageViewModel : BaseTabViewModel
{
    private const int CAMERA_TIME_CHECK_SECONDS = 5;
    private CancellationTokenSource _cameraCancellationTokenSource;
    private readonly LibVLC _libVLC = new(enableDebugLogs: false);

    private readonly IMapperService _mapperService;
    private readonly ICamerasService _camerasService;

    public CamerasPageViewModel(
        INavigationService navigationService,
        IMapperService mapperService,
        ICamerasService camerasService)
        : base(navigationService)
    {
        _mapperService = mapperService;
        _camerasService = camerasService;
        DataState = EPageState.LoadingSkeleton;

        Title = "Cameras";
        _camerasService.AllCamerasChanged += OnAllCamerasChanged;
    }

    #region -- Public properties --

    private MediaPlayer _mediaPlayer;
    public MediaPlayer MediaPlayer
    {
        get => _mediaPlayer;
        set => SetProperty(ref _mediaPlayer, value);
    }

    private ObservableCollection<CameraBindableModel> _cameras;
    public ObservableCollection<CameraBindableModel> Cameras
    {
        get => _cameras;
        set => SetProperty(ref _cameras, value);
    }

    private CameraBindableModel _selectedCamera;
    public CameraBindableModel SelectedCamera
    {
        get => _selectedCamera;
        set => SetProperty(ref _selectedCamera, value);
    }

    private bool _isCamerasRefreshing;
    public bool IsCamerasRefreshing
    {
        get => _isCamerasRefreshing;
        set => SetProperty(ref _isCamerasRefreshing, value);
    }

    private VLCState _videoState = VLCState.NothingSpecial;
    public VLCState VideoState
    {
        get => _videoState;
        set => SetProperty(ref _videoState, value);
    }

    private ICommand _selectCameraCommand;
    public ICommand SelectCameraCommand => _selectCameraCommand ??= SingleExecutionCommand.FromFunc<CameraBindableModel>(OnSelectCameraCommandAsync);

    private ICommand _refreshCamerasCommand;
    public ICommand RefreshCamerasCommand => _refreshCamerasCommand ??= SingleExecutionCommand.FromFunc(OnRefreshCamerasCommandAsync);

    private ICommand _tryAgainCommand;
    public ICommand TryAgainCommand => _tryAgainCommand ??= SingleExecutionCommand.FromFunc(OnTryAgainCommandAsync);

    #endregion

    #region -- Overrides --

    public override async void OnAppearing()
    {
        base.OnAppearing();

        _cameraCancellationTokenSource = new();

        await LoadCamerasAndChangeStateAsync();

        _ = StartConnectionRunnerForEachCameraAsync().ConfigureAwait(false);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.PropertyName is nameof(SelectedCamera))
        {
            PlayCurrentVideoAsync().ConfigureAwait(false);
        }
    }

    private Task PlayCurrentVideoAsync()
    {
        Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedCamera?.VideoUrl) || !SelectedCamera.IsConnected)
                {
                    StopVideo();
                }
                else
                {
                    if (MediaPlayer is null)
                    {
                        MediaPlayer = new(_libVLC) { EnableHardwareDecoding = true };
                        MediaPlayer.EncounteredError += OnMediaPlayerEncounteredError;
                        MediaPlayer.Opening += OnMediaPlayerStateChanged;
                        MediaPlayer.EndReached += OnMediaPlayerStateChanged;
                        MediaPlayer.Playing += OnMediaPlayerStateChanged;
                        MediaPlayer.NothingSpecial += OnMediaPlayerStateChanged;
                        MediaPlayer.Stopped += OnMediaPlayerStateChanged;
                        MediaPlayer.PositionChanged += OnMediaPlayerPositionChanged;
                    }

                    using var media = new Media(_libVLC, SelectedCamera.VideoUrl, FromType.FromLocation);

                    MediaPlayer?.Play(media);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(PlayCurrentVideoAsync)}: {ex.Message}");
            }
        });

        return Task.CompletedTask;
    }

    private void OnMediaPlayerPositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
    {
        VideoState = VLCState.Playing;
    }

    private void OnMediaPlayerStateChanged(object sender, EventArgs e)
    {
        if (MediaPlayer.State is VLCState.Playing)
        {
            VideoState = VLCState.Opening;
        }
        else
        {
            VideoState = MediaPlayer.State;
        }
    }

    private void StopVideo()
    {
        if (MediaPlayer is not null)
        {
            MediaPlayer.EncounteredError -= OnMediaPlayerEncounteredError;
            MediaPlayer.Opening -= OnMediaPlayerStateChanged;
            MediaPlayer.EndReached -= OnMediaPlayerStateChanged;
            MediaPlayer.Playing -= OnMediaPlayerStateChanged;
            MediaPlayer.NothingSpecial -= OnMediaPlayerStateChanged;
            MediaPlayer.Stopped -= OnMediaPlayerStateChanged;
            MediaPlayer.PositionChanged -= OnMediaPlayerPositionChanged;
            MediaPlayer.Pause();
            MediaPlayer.Stop();
            MediaPlayer.Dispose();
            MediaPlayer = null;
            VideoState = VLCState.NothingSpecial;
        }
    }

    private void OnMediaPlayerEncounteredError(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => Toast.Make($"{Strings.CannotPlayVideo}").Show());
    }

    public override void OnResume()
    {
        base.OnResume();

        OnAppearing();
    }

    public override void OnSleep()
    {
        base.OnSleep();

        OnDisappearing();
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();

        _cameraCancellationTokenSource?.Cancel();

        StopVideo();
    }

    protected override async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            if (!IsDataLoading && DataState != EPageState.Complete)
            {
                DataState = EPageState.LoadingSkeleton;

                await LoadCamerasAndChangeStateAsync();
            }
        }
        else
        {
            IsCamerasRefreshing = false;
            StopVideo();
            DataState = EPageState.NoInternet;
        }
    }

    #endregion

    #region -- Private helpers --

    private async void OnAllCamerasChanged(object sender, EventArgs e)
    {
        await LoadCamerasAndChangeStateAsync();
    }

    private async Task OnTryAgainCommandAsync()
    {
        if (!IsDataLoading)
        {
            DataState = EPageState.NoInternetLoader;

            var executionTime = TimeSpan.FromSeconds(Constants.Limits.TIME_TO_ATTEMPT_UPDATE_IN_SECONDS);

            var isDataLoaded = await TaskRepeater.RepeatAsync(LoadCamerasAsync, executionTime);

            if (IsInternetConnected)
            {
                DataState = isDataLoaded ? EPageState.Complete : EPageState.Empty;

                if (isDataLoaded)
                {
                    _ = PlayCurrentVideoAsync().ConfigureAwait(false);
                }
                else
                {
                    StopVideo();
                }
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
    }

    private Task OnSelectCameraCommandAsync(CameraBindableModel selectedCamera)
    {
        SelectCamera(selectedCamera);

        return Task.CompletedTask;
    }

    private async Task OnRefreshCamerasCommandAsync()
    {
        if (!IsDataLoading)
        {
            await LoadCamerasAndChangeStateAsync();

            IsCamerasRefreshing = false;
        }
    }

    private async Task LoadCamerasAndChangeStateAsync()
    {
        if (IsInternetConnected)
        {
            var isDataLoaded = await LoadCamerasAsync();

            if (IsInternetConnected)
            {
                DataState = isDataLoaded ? EPageState.Complete : EPageState.Empty;

                if (isDataLoaded)
                {
                    _ = PlayCurrentVideoAsync().ConfigureAwait(false);
                }
                else
                {
                    StopVideo();
                }

                DataState = Cameras.Any()
                    ? EPageState.Complete
                    : EPageState.Empty;
            }
            else
            {
                DataState = EPageState.NoInternet;
            }
        }
        else
        {
            DataState = EPageState.NoInternet;
        }
    }

    private Task StartConnectionRunnerForEachCameraAsync()
    {
        foreach (var cam in Cameras)
        {
            StartRunnerAsync(cam).ConfigureAwait(false);
        }

        return Task.CompletedTask;
    }

    private async Task StartRunnerAsync(CameraBindableModel camera)
    {
        try
        {
            while (!_cameraCancellationTokenSource.IsCancellationRequested)
            {
                var isConnectedResponse = await _camerasService.CheckCameraConnection(camera, _cameraCancellationTokenSource.Token).ConfigureAwait(false);

                if (!_cameraCancellationTokenSource.IsCancellationRequested)
                {
                    if (isConnectedResponse.IsSuccess && isConnectedResponse.Result)
                    {
                        if (SelectedCamera == camera && !camera.IsConnected)
                        {
                            _ = PlayCurrentVideoAsync().ConfigureAwait(false);
                        }

                        camera.IsConnected = true;
                    }
                    else if (camera.IsConnected)
                    {
                        if (SelectedCamera == camera && camera.IsConnected)
                        {
                            StopVideo();
                        }

                        camera.IsConnected = false;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(CAMERA_TIME_CHECK_SECONDS), _cameraCancellationTokenSource.Token);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(StartRunnerAsync)}: {ex.Message}");
        }
    }

    private async Task<bool> LoadCamerasAsync()
    {
        bool isLoaded = false;

        if (IsInternetConnected)
        {
            var resultOfGettingCameras = await _camerasService.GetCamerasAsync();

            if (resultOfGettingCameras.IsSuccess)
            {
                var cameras = _mapperService.MapRange<CameraBindableModel>(resultOfGettingCameras.Result.Where(x => x.IsShown), (m, vm) =>
                {
                    vm.TapCommand = SelectCameraCommand;

                    if (Cameras is not null && Cameras.Any() && Cameras.Any(x => x.VideoUrl == vm.VideoUrl))
                    {
                        var cam = Cameras.FirstOrDefault(x => x.VideoUrl == vm.VideoUrl);
                        vm.IsConnected = cam.IsConnected;
                    }
                });

                Cameras = new(cameras);

                var camera = (SelectedCamera is null || !SelectedCamera.IsShown)
                    ? Cameras.FirstOrDefault()
                    : Cameras.FirstOrDefault(x => x.Id == SelectedCamera.Id) ?? Cameras.FirstOrDefault();

                SelectCamera(camera);

                isLoaded = true;
            }
        }

        return isLoaded;
    }

    private void SelectCamera(CameraBindableModel selectedCamera)
    {
        if (SelectedCamera is not null)
        {
            SelectedCamera.IsSelected = false;
        }

        if (selectedCamera is not null)
        {
            selectedCamera.IsSelected = true;
        }

        SelectedCamera = selectedCamera;
    }

    #endregion
}
