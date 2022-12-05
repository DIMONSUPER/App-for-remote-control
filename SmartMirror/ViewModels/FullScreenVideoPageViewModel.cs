using CommunityToolkit.Maui.Alerts;
using LibVLCSharp.Shared;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Helpers.Events;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Mapper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class FullScreenVideoPageViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public FullScreenVideoPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator) 
            : base(navigationService)
        {
            _eventAggregator = eventAggregator;
        }

        #region -- Public properties --

        private CameraBindableModel _camera = new() { Name = "Default" };
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

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CAMERA, out CameraBindableModel camera))
            {
                Camera = camera;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnCloseFullScreenCommandAsync()
        {
            _eventAggregator.GetEvent<CloseFullScreenVideoEvent>().Publish();

            return Task.CompletedTask;
        }

        #endregion
    }
}