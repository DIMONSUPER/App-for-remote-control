using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;

namespace SmartMirror.Controls
{
    public class Video : View, IVideoController
    {
        private IDispatcherTimer _timer;

        public Video()
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(Constants.Limits.VIDEO_STATUS_UPDATE_INTERVAL);
            _timer.Tick += OnUpdateVideoStatusTimerTick;
            _timer.Start();
        }

        ~Video() => _timer.Tick -= OnUpdateVideoStatusTimerTick;

        #region -- Public properties --

        public event EventHandler UpdateStatus;

        public event EventHandler<VideoPositionEventArgs> PlayRequested;

        public event EventHandler<VideoPositionEventArgs> PauseRequested;

        public event EventHandler<VideoPositionEventArgs> StopRequested;

        public static readonly BindableProperty IsControlPanelEnabledProperty = BindableProperty.Create(
            propertyName: nameof(IsControlPanelEnabled),
            returnType: typeof(bool),
            declaringType: typeof(Video),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsControlPanelEnabled
        {
            get => (bool)GetValue(IsControlPanelEnabledProperty);
            set => SetValue(IsControlPanelEnabledProperty, value);
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: nameof(Source),
            returnType: typeof(string),
            declaringType: typeof(Video),
            defaultBindingMode: BindingMode.OneWay);

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly BindableProperty IsAutoPlayProperty = BindableProperty.Create(
            propertyName: nameof(IsAutoPlay),
            returnType: typeof(bool),
            declaringType: typeof(Video),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsAutoPlay
        {
            get { return (bool)GetValue(IsAutoPlayProperty); }
            set { SetValue(IsAutoPlayProperty, value); }
        }

        public static readonly BindableProperty PositionProperty = BindableProperty.Create(
            propertyName: nameof(Position),
            returnType: typeof(TimeSpan),
            declaringType: typeof(Video),
            defaultValue: new TimeSpan(),
            defaultBindingMode: BindingMode.OneWay);

        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        private static readonly BindablePropertyKey StatusPropertyKey = BindableProperty.CreateReadOnly(
            propertyName: nameof(Status),
            returnType: typeof(EVideoStatus),
            declaringType: typeof(Video),
            defaultValue: EVideoStatus.NotReady,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty StatusProperty = StatusPropertyKey.BindableProperty;

        public EVideoStatus Status => (EVideoStatus)GetValue(StatusProperty);

        #endregion

        #region -- IVideoController implementation --

        EVideoStatus IVideoController.Status
        {
            get => Status; 
            set => SetValue(StatusPropertyKey, value);
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

        private void OnUpdateVideoStatusTimerTick(object sender, EventArgs e)
        {
            UpdateStatus?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(UpdateStatus));
        }

        #endregion
    }
}