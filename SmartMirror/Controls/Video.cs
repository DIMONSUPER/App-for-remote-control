using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Interfaces;

namespace SmartMirror.Controls
{
    public class Video : View, IVideoController
    {
        #region -- Public properties --

        public event EventHandler UpdateStatus;

        public event EventHandler PlayRequested;

        public event EventHandler PauseRequested;

        public event EventHandler StopRequested;

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

        private static readonly BindablePropertyKey LoadingStatusPropertyKey = BindableProperty.CreateReadOnly(
            propertyName: nameof(LoadingStatus),
            returnType: typeof(EVideoLoadingStatus),
            declaringType: typeof(Video),
            defaultValue: EVideoLoadingStatus.Unprepared,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty LoadingStatusProperty = LoadingStatusPropertyKey.BindableProperty;

        public EVideoLoadingStatus LoadingStatus => (EVideoLoadingStatus)GetValue(LoadingStatusProperty);

        #endregion

        #region -- IVideoController implementation --

        EVideoLoadingStatus IVideoController.LoadingStatus
        {
            get => LoadingStatus; 
            set => SetValue(LoadingStatusPropertyKey, value);
        }

        #endregion

        #region -- Public helpers --

        public void Play()
        {
            PlayRequested?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(PlayRequested));
        }

        public void Pause()
        {
            PauseRequested?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(PauseRequested));
        }

        public void Stop()
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(StopRequested));
        }

        #endregion
    }
}