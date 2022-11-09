using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Cameras;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class CameraSettingsDialogViewModel : BaseDialogViewModel
    {
        private readonly ICamerasService _camerasService;
        private bool _isInitializing = true;

        public CameraSettingsDialogViewModel(
            IBlurService blurService,
            ICamerasService camerasService)
            : base(blurService)
        {
            _camerasService = camerasService;
        }

        #region -- Public properties --

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isShownInCameras;
        public bool IsShownInCameras
        {
            get => _isShownInCameras;
            set => SetProperty(ref _isShownInCameras, value);
        }

        private CameraModel _cameraBindableModel;
        public CameraModel CameraModel
        {
            get => _cameraBindableModel;
            set => SetProperty(ref _cameraBindableModel, value);
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

        private ICommand _removeCameraCommand;
        public ICommand RemoveCameraCommand => _removeCameraCommand ??= SingleExecutionCommand.FromFunc(OnRemoveCameraCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CAMERA, out ImageAndTitleBindableModel camera))
            {
                Title = camera.Name;

                if (camera.Model is CameraModel cameraModel)
                {
                    CameraModel = cameraModel;
                    IsShownInCameras = CameraModel.IsShown;
                }
            }

            _isInitializing = false;
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (!_isInitializing && args.PropertyName is nameof(IsShownInCameras))
            {
                CameraModel.IsShown = IsShownInCameras;
                await _camerasService.UpdateCameraAsync(CameraModel);
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnRemoveCameraCommandAsync()
        {
            RequestClose.Invoke(new DialogParameters()
            {
                { Constants.DialogsParameterKeys.RESULT, true },
            });

            return Task.CompletedTask;
        }

        private Task OnCloseCommandAsync()
        {
            RequestClose.Invoke();

            return Task.CompletedTask;
        }

        #endregion
    }
}
