using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Blur;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class CameraSettingsDialogViewModel : BaseDialogViewModel
    {
        public CameraSettingsDialogViewModel(IBlurService blurService)
            : base(blurService)
        {
        }

        #region -- Public properties --

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
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
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnRemoveCameraCommandAsync()
        {
            RequestClose.Invoke(new DialogParameters()
            {
                { Constants.DialogsParameterKeys.SHOW_CONFIRM_DIALOG, true },
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
