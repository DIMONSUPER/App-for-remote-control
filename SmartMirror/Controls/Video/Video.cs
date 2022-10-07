using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;

namespace SmartMirror.Controls.Video
{
    public class Video : View, IVideoController
    {
        private IDispatcherTimer _timer;

        public Video()
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        ~Video() => _timer.Tick -= OnTimerTick;

        #region -- Public properties --

        public event EventHandler UpdateStatus;

        public event EventHandler<VideoPositionEventArgs> PlayRequested;

        public event EventHandler<VideoPositionEventArgs> PauseRequested;

        public event EventHandler<VideoPositionEventArgs> StopRequested;

        public static readonly BindableProperty AreTransportControlsEnabledProperty = BindableProperty.Create(
            propertyName: nameof(AreTransportControlsEnabled),
            returnType: typeof(bool),
            declaringType: typeof(Video),
            defaultValue: true);

        public bool AreTransportControlsEnabled
        {
            get => (bool)GetValue(AreTransportControlsEnabledProperty);
            set => SetValue(AreTransportControlsEnabledProperty, value);
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: nameof(Source),
            returnType: typeof(string),
            declaringType: typeof(Video));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create(
            propertyName: nameof(AutoPlay),
            returnType: typeof(bool),
            declaringType: typeof(Video),
            defaultValue: true);

        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }

        public static readonly BindableProperty PositionProperty = BindableProperty.Create(
            propertyName: nameof(Position),
            returnType: typeof(TimeSpan),
            declaringType: typeof(Video),
            defaultValue: new TimeSpan());

        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion

        private static readonly BindablePropertyKey StatusPropertyKey = BindableProperty.CreateReadOnly(
            propertyName: nameof(Status),
            returnType: typeof(EVideoStatus),
            declaringType: typeof(Video),
            defaultValue: EVideoStatus.NotReady);

        public static readonly BindableProperty StatusProperty = StatusPropertyKey.BindableProperty;

        public EVideoStatus Status
        {
            get { return (EVideoStatus)GetValue(StatusProperty); }
        }

        private static readonly BindablePropertyKey DurationPropertyKey = BindableProperty.CreateReadOnly(
            propertyName: nameof(Duration),
            returnType: typeof(TimeSpan),
            declaringType: typeof(Video),
            defaultValue: new TimeSpan());

        public static readonly BindableProperty DurationProperty = DurationPropertyKey.BindableProperty;

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
        }

        #region -- IVideoController implementation --

        EVideoStatus IVideoController.Status
        {
            get { return Status; }
            set { SetValue(StatusPropertyKey, value); }
        }

        TimeSpan IVideoController.Duration
        {
            get { return Duration; }
            set { SetValue(DurationPropertyKey, value); }
        }

        #endregion

        #region -- Public helpers --

        public void Play()
        {
            var args = new VideoPositionEventArgs(Position);

            PlayRequested?.Invoke(this, args);
            Handler?.Invoke(nameof(PlayRequested), args);
        }

        public void Pause()
        {
            var args = new VideoPositionEventArgs(Position);

            PauseRequested?.Invoke(this, args);
            Handler?.Invoke(nameof(PauseRequested), args);
        }

        public void Stop()
        {
            var args = new VideoPositionEventArgs(Position);

            StopRequested?.Invoke(this, args);
            Handler?.Invoke(nameof(StopRequested), args);
        }

        #endregion

        #region -- Private helpres --

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateStatus?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(UpdateStatus));
        }

        #endregion
    }
}