using SmartMirror.Helpers;
using SmartMirror.Services.Blur;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class AddNewCameraDialogViewModel : BaseDialogViewModel
    {
        public AddNewCameraDialogViewModel(
            IBlurService blurService)
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

        private string _ipAddress;
        public string IPAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private ICommand _addCameraCommand;
        public ICommand AddCameraCommand => _addCameraCommand ??= SingleExecutionCommand.FromFunc(OnAddCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
            {
                Title = title;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnAddCommandAsync()
        {
            RequestClose.Invoke(new DialogParameters
            {
                { Constants.DialogsParameterKeys.IP_ADDRESS, IPAddress },
                { Constants.DialogsParameterKeys.PASSWORD, Password },
            });

            return Task.CompletedTask;
        }

        #endregion
    }
}
