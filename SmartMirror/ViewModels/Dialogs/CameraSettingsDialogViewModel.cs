using System.ComponentModel;
using System.Windows.Input;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Resources.Strings;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Services.Cameras;

namespace SmartMirror.ViewModels.Dialogs
{
    public class CameraSettingsDialogViewModel : BaseDialogViewModel
    {
        private readonly ICamerasService _camerasService;
        private readonly IDialogService _dialogService;
        private bool _isInitializing = true;

        public CameraSettingsDialogViewModel(
            IBlurService blurService,
            ICamerasService camerasService,
            IDialogService dialogService,
            IKeyboardService keyboardService)
            : base(blurService, keyboardService)
        {
            _camerasService = camerasService;
            _dialogService = dialogService;
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

        private bool _isReceiveNotifications;
        public bool IsReceiveNotifications
        {
            get => _isReceiveNotifications;
            set => SetProperty(ref _isReceiveNotifications, value);
        }

        private CameraBindableModel _cameraModel;
        public CameraBindableModel CameraModel
        {
            get => _cameraModel;
            set => SetProperty(ref _cameraModel, value);
        }

        private ICommand _removeCameraCommand;
        public ICommand RemoveCameraCommand => _removeCameraCommand ??= SingleExecutionCommand.FromFunc(OnRemoveCameraCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.CAMERA, out ImageAndTitleBindableModel camera))
            {
                Title = camera.Name;

                if (camera.Model is CameraBindableModel cameraModel)
                {
                    CameraModel = cameraModel;
                    IsShownInCameras = CameraModel.IsShown;
                    IsReceiveNotifications = CameraModel.IsReceiveNotifications;
                }
            }

            _isInitializing = false;
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (!_isInitializing && args.PropertyName is nameof(IsShownInCameras) or nameof(IsReceiveNotifications))
            {
                CameraModel.IsShown = IsShownInCameras;
                CameraModel.IsReceiveNotifications = IsReceiveNotifications;
                await _camerasService.UpdateCameraAsync(CameraModel);
            }
        }

        #endregion

        #region -- Private helpers --

        private async void OnRemoveConfirmed()
        {
            await _camerasService.RemoveCameraAsync(CameraModel);
        }

        private async Task OnRemoveCameraCommandAsync()
        {
            var dialogResult = await _dialogService.ShowDialogAsync(nameof(Views.Dialogs.ConfirmDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, Strings.AreYouSure },
                { Constants.DialogsParameterKeys.DESCRIPTION, Strings.TheCameraWillBeRemoved },
                { Constants.DialogsParameterKeys.CONFIRM_ACTION, (Action)OnRemoveConfirmed },
            });

            if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool isConfirmed) && isConfirmed)
            {
                RequestClose.Invoke(new DialogParameters
                {
                    { Constants.DialogsParameterKeys.RESULT, true },
                });
            }
        }

        #endregion
    }
}
