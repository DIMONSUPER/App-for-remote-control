using CommunityToolkit.Maui.Alerts;
using LibVLCSharp.Shared;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Cameras;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class FullScreenCameraPageViewModel : BaseViewModel
    {
        private readonly ICamerasService _camerasService;

        private readonly LibVLC _libVLC = new(enableDebugLogs: false);
        private CancellationTokenSource _cameraCancellationTokenSource;

        public FullScreenCameraPageViewModel(
            INavigationService navigationService,
            ICamerasService camerasService) 
            : base(navigationService)
        {
            _camerasService = camerasService;
        }

        #region -- Public properties --

        private CameraBindableModel _camera;
        public CameraBindableModel Camera
        {
            get => _camera;
            set => SetProperty(ref _camera, value);
        }

        private VLCState _videoState = VLCState.NothingSpecial;
        public VLCState VideoState
        {
            get => _videoState;
            set => SetProperty(ref _videoState, value);
        }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set => SetProperty(ref _mediaPlayer, value);
        }

        private ICommand _closeFullScreenCommand;
        public ICommand CloseFullScreenCommand => _closeFullScreenCommand ??= SingleExecutionCommand.FromFunc(OnCloseFullScreenCommandAsync);

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CAMERA, out CameraBindableModel camera))
            {
                Camera = camera;
            }
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            _cameraCancellationTokenSource = new();

            _ = StartRunnerAsync().ConfigureAwait(false);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            _cameraCancellationTokenSource?.Cancel();

            StopVideo();
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

        #endregion

        #region -- Private helpers --

        private Task PlayCurrentVideoAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Camera?.VideoUrl) || !Camera.IsConnected)
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

                        using var media = new Media(_libVLC, Camera.VideoUrl, FromType.FromLocation);

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

        private async Task StartRunnerAsync()
        {
            try
            {
                while (!_cameraCancellationTokenSource.IsCancellationRequested)
                {
                    var isConnectedResponse = await _camerasService.CheckCameraConnection(Camera, _cameraCancellationTokenSource.Token).ConfigureAwait(false);

                    if (!_cameraCancellationTokenSource.IsCancellationRequested)
                    {
                        if (isConnectedResponse.IsSuccess && isConnectedResponse.Result)
                        {
                            if (!Camera.IsConnected)
                            {
                                _ = PlayCurrentVideoAsync().ConfigureAwait(false);
                            }

                            Camera.IsConnected = true;  
                        }
                        else if (Camera.IsConnected)
                        {   
                            StopVideo();

                            Camera.IsConnected = false;
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

        private void OnMediaPlayerPositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            VideoState = VLCState.Playing;
        }

        private void OnMediaPlayerStateChanged(object sender, EventArgs e)
        {   
            VideoState = MediaPlayer.State is VLCState.Playing
                ? VLCState.Opening
                : MediaPlayer.State;
        }

        private void OnMediaPlayerEncounteredError(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => Toast.Make($"{Strings.CannotPlayVideo}").Show());
        }

        private Task OnCloseFullScreenCommandAsync() => NavigationService.GoBackAsync();

        #endregion
    }
}