using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using LibVLCSharp.Shared;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Helpers.Events;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;

namespace SmartMirror.ViewModels.Tabs.Pages;

public class CamerasPageViewModel : BaseTabViewModel
{
    private const int COLOR_PROPERTY_CHANGED_THROTTLE = 400;

    private CancellationTokenSource _cameraCancellationTokenSource;
    private readonly LibVLC _libVLC = new(enableDebugLogs: false);
    private event EventHandler<double> _videoColorPropertyChanged;

    private readonly IEventAggregator _eventAggregator;
    private readonly IMapperService _mapperService;
    private readonly ICamerasService _camerasService;

    public CamerasPageViewModel(
        INavigationService navigationService,
        IEventAggregator eventAggregator,
        IMapperService mapperService,
        ICamerasService camerasService)
        : base(navigationService)
    {
        _eventAggregator = eventAggregator;
        _mapperService = mapperService;
        _camerasService = camerasService;
        DataState = EPageState.LoadingSkeleton;

        Title = "Cameras";

        _camerasService.AllCamerasChanged += OnAllCamerasChanged;
        SubscribeToVideoColorPropertyChanged();
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

    private bool _isMuted;
    public bool IsMuted
    {
        get => _isMuted;
        set => SetProperty(ref _isMuted, value);
    }

    private bool _isHighQualityOn;
    public bool IsHighQualityOn
    {
        get => _isHighQualityOn;
        set => SetProperty(ref _isHighQualityOn, value);
    }

    private double _brightness = 50;
    public double Brightness
    {
        get => _brightness;
        set => SetProperty(ref _brightness, value);
    }

    private double _contrast = 50;
    public double Contrast
    {
        get => _contrast;
        set => SetProperty(ref _contrast, value);
    }

    private double _hue = 50;
    public double Hue
    {
        get => _hue;
        set => SetProperty(ref _hue, value);
    }

    private double _saturation = 50;
    public double Saturation
    {
        get => _saturation;
        set => SetProperty(ref _saturation, value);
    }

    private ICommand _selectCameraCommand;
    public ICommand SelectCameraCommand => _selectCameraCommand ??= SingleExecutionCommand.FromFunc<CameraBindableModel>(OnSelectCameraCommandAsync);

    private ICommand _refreshCamerasCommand;
    public ICommand RefreshCamerasCommand => _refreshCamerasCommand ??= SingleExecutionCommand.FromFunc(OnRefreshCamerasCommandAsync);

    private ICommand _openVideoInFullScreenCommand;
    public ICommand OpenVideoInFullScreenCommand => _openVideoInFullScreenCommand ??= SingleExecutionCommand.FromFunc(OnOpenVideoInFullScreenCommandAsync);

    private ICommand _takeSnapshotCommand;
    public ICommand TakeSnapshotCommand => _takeSnapshotCommand ??= SingleExecutionCommand.FromFunc(OnTakeSnapshotCommandAsync);

    private ICommand _muteVideoCommand;
    public ICommand MuteVideoCommand => _muteVideoCommand ??= SingleExecutionCommand.FromFunc(OnMuteVideoCommandAsync);

    private ICommand _switchVideoQualityCommand;
    public ICommand SwitchVideoQualityCommand => _switchVideoQualityCommand ??= SingleExecutionCommand.FromFunc(OnSwitchVideoQualityCommandAsync);

    #endregion

    #region -- Overrides --

    public override async void OnAppearing()
    {
        base.OnAppearing();

        _cameraCancellationTokenSource = new();

        await LoadCamerasAndChangeStateAsync();

        _ = StartConnectionRunnerForEachCameraAsync().ConfigureAwait(false);
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.PropertyName is nameof(SelectedCamera))
        {
            _ = PlayCurrentVideoAsync().ConfigureAwait(false);

            await SetCameraSettingsAsync();
        }
        else if (args.PropertyName is nameof(Brightness))
        {
            _videoColorPropertyChanged?.Invoke(this, Brightness);
        }
        else if (args.PropertyName is nameof(Contrast))
        {
            _videoColorPropertyChanged?.Invoke(this, Contrast);
        }
        else if (args.PropertyName is nameof(Hue))
        {
            _videoColorPropertyChanged?.Invoke(this, Hue);
        }
        else if (args.PropertyName is nameof(Saturation))
        {
            _videoColorPropertyChanged?.Invoke(this, Saturation);
        }
    }

    private async Task SetCameraSettingsAsync()
    {
        if (SelectedCamera is not null && SelectedCamera.IsConnected)
        {
            var videoColorResponse = await _camerasService.GetVideoColorConfigsAsync(SelectedCamera);

            if (videoColorResponse.IsSuccess)
            {
                Brightness = videoColorResponse.Result.Params.Table[0].Brightness;
                Contrast = videoColorResponse.Result.Params.Table[0].Contrast;
                Hue = videoColorResponse.Result.Params.Table[0].Hue;
                Saturation = videoColorResponse.Result.Params.Table[0].Saturation;
            }

            var cameraConfigsResponse = await _camerasService.GetCameraConfigsAsync(SelectedCamera);

            if (cameraConfigsResponse.IsSuccess)
            {
                SelectedCamera.IsMuted = !cameraConfigsResponse.Result.Params.Table.MainFormat[0].AudioEnable;
            }
        }
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

                    await SetCameraSettingsAsync();
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
            //StartRunnerAsync(cam).ConfigureAwait(false);
        }

        return Task.CompletedTask;
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
                        MediaPlayer = new(_libVLC) { EnableHardwareDecoding = true, NetworkCaching = 0, Volume = 0, };
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
        if (MediaPlayer?.State is VLCState.Playing)
        {
            VideoState = VLCState.Opening;
        }
        else
        {
            VideoState = MediaPlayer?.State ?? VLCState.Ended;
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
                            await SetCameraSettingsAsync();
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

                    await Task.Delay(TimeSpan.FromSeconds(Constants.Limits.CAMERA_TIME_CHECK_SECONDS), _cameraCancellationTokenSource.Token);
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
                var cameras = _mapperService.MapRange<CameraBindableModel, CameraBindableModel>(resultOfGettingCameras.Result.Where(x => x.IsShown), (m, vm) =>
                {
                    vm.Name = string.IsNullOrEmpty(m.Name)
                        ? m.IpAddress
                        : m.Name;

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

    private Task OnOpenVideoInFullScreenCommandAsync()
    {
        if (SelectedCamera is not null && SelectedCamera.IsConnected)
        {
            StopVideo();

            SelectedCamera.IsConnected = false;

            _eventAggregator.GetEvent<OpenFullScreenCameraEvent>().Publish(SelectedCamera);
        }

        return Task.CompletedTask;
    }

    private void SubscribeToVideoColorPropertyChanged()
    {
        var _subscription = Observable.FromEventPattern<double>(
            handler => _videoColorPropertyChanged += handler,
            handler => _videoColorPropertyChanged -= handler)
            .Throttle(TimeSpan.FromMilliseconds(COLOR_PROPERTY_CHANGED_THROTTLE))
            .ObserveOn(SynchronizationContext.Current)
            .Select(eventPattern => eventPattern)
            .DistinctUntilChanged()
            .Subscribe(async query => await SetVideoColorPropertyAsync());
    }

    private Task OnTakeSnapshotCommandAsync()
    {
        var storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);

        string path = Path.Combine(storagePath.ToString(), $"Snapshot-{DateTime.Now:yyyy-MM-dd_HH:mm:ss}.jpeg");

        var isSnapshotTaken = MediaPlayer.TakeSnapshot(0, path, 0, 0);

        return Task.CompletedTask;
    }

    private async Task SetVideoColorPropertyAsync()
    {
        if (SelectedCamera is null || !SelectedCamera.IsConnected) return;

        var videoColorResponse = await _camerasService.GetVideoColorConfigsAsync(SelectedCamera);

        if (videoColorResponse.IsSuccess)
        {
            var table = videoColorResponse.Result.Params.Table;

            var needsEditing = table[0].Brightness != (int)Brightness;
            table[0].Brightness = (int)Brightness;

            needsEditing |= table[0].Contrast != (int)Contrast;
            table[0].Contrast = (int)Contrast;

            needsEditing |= table[0].Hue != (int)Hue;
            table[0].Hue = (int)Hue;

            needsEditing |= table[0].Saturation != (int)Saturation;
            table[0].Saturation = (int)Saturation;

            if (needsEditing)
            {
                var setVideoColorResponse = await _camerasService.SetVideoColorConfigsAsync(SelectedCamera, table);
            }
        }
    }

    private async Task OnMuteVideoCommandAsync()
    {
        var cameraConfigsResponse = await _camerasService.GetCameraConfigsAsync(SelectedCamera);

        if (cameraConfigsResponse.IsSuccess)
        {
            cameraConfigsResponse.Result.Params.Table.MainFormat[0].AudioEnable = SelectedCamera.IsMuted;

            var setCameraConfigsResponse = await _camerasService.SetCameraConfigsAsync(SelectedCamera, cameraConfigsResponse.Result.Params.Table);

            if (setCameraConfigsResponse.IsSuccess)
            {
                SelectedCamera.IsMuted = !SelectedCamera.IsMuted;
            }
        }
    }

    private async Task OnSwitchVideoQualityCommandAsync()
    {
        if (SelectedCamera is not null)
        {
            SelectedCamera.SubType = SelectedCamera.SubType is 0 ? 1 : 0;

            await _camerasService.UpdateCameraAsync(SelectedCamera);
        }
    }

    #endregion
}