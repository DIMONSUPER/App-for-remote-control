using SmartMirror.Enums;
using SmartMirror.Interfaces;
using System.Runtime.CompilerServices;

namespace SmartMirror.Controls
{
    public class Video : View, IVideoController
    {
        ~Video()
        {
            Handler?.DisconnectHandler();
        }

        #region -- Public properties --

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

        private static readonly BindablePropertyKey LoadingStatePropertyKey = BindableProperty.CreateReadOnly(
            propertyName: nameof(LoadingState),
            returnType: typeof(EVideoLoadingState),
            declaringType: typeof(Video),
            defaultValue: EVideoLoadingState.Unprepared);

        public static readonly BindableProperty LoadingStateProperty = LoadingStatePropertyKey.BindableProperty;

        public EVideoLoadingState LoadingState => (EVideoLoadingState)GetValue(LoadingStateProperty);

        public static readonly BindableProperty ActionProperty = BindableProperty.Create(
            propertyName: nameof(Action),
            returnType: typeof(EVideoAction),
            declaringType: typeof(Video), 
            defaultBindingMode: BindingMode.OneWay);

        public EVideoAction Action
        {
            get => (EVideoAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        #endregion

        #region -- IVideoController implementation --

        EVideoLoadingState IVideoController.LoadingState
        {
            get => LoadingState;
            set => SetValue(LoadingStatePropertyKey, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(Action))
            {
                Action videoAction = Action switch
                {
                    EVideoAction.Play => Play,
                    EVideoAction.Pause => Pause,
                    EVideoAction.Stop => Stop,
                    _ => null,
                };

                videoAction?.Invoke();
            }
        } 

        #endregion

        #region -- Private helpers --

        private void Play()
        {
            PlayRequested?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(PlayRequested));
        }

        private void Pause()
        {
            PauseRequested?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(PauseRequested));
        }

        private void Stop()
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
            Handler?.Invoke(nameof(StopRequested));
        }

        #endregion
    }
}