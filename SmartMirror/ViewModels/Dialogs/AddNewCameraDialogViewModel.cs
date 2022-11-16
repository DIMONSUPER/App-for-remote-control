using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Cameras;
using System.Windows.Input;
using SmartMirror.Views.Dialogs;

namespace SmartMirror.ViewModels.Dialogs
{
    public class AddNewCameraDialogViewModel : BaseDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly ICamerasService _camerasService;

        public AddNewCameraDialogViewModel(
            IBlurService blurService,
            IDialogService dialogService,
            ICamerasService camerasService)
            : base(blurService)
        {
            _dialogService = dialogService;
            _camerasService = camerasService;
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

        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private bool _isIpAddressCorrect;
        public bool IsIpAddressCorrect
        {
            get => _isIpAddressCorrect;
            set => SetProperty(ref _isIpAddressCorrect, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isIpAddressEntryFocused;
        public bool IsIpAddressEntryFocused
        {
            get => _isIpAddressEntryFocused;
            set => SetProperty(ref _isIpAddressEntryFocused, value);
        }

        private ICommand _addCameraCommand;
        public ICommand AddCameraCommand => _addCameraCommand ??= SingleExecutionCommand.FromFunc(OnAddCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
            {
                Title = title;

                await Task.Delay(FOCUS_DELAY);
                IsIpAddressEntryFocused = true;
            }
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();

            IsIpAddressEntryFocused = false;
        }

        #endregion

        #region -- Private helpers --

        private async Task<bool> VerifyIpAddressCorrectAsync()
        {
            var virifyIpAddressResponse = await _camerasService.VerifyCameraIPAddressAsync(IPAddress);

            return virifyIpAddressResponse.IsSuccess && virifyIpAddressResponse.Result;
        }

        private async Task<bool> VerifyCameraConnectionAsync()
        {
            var virifyIpAddressResponse = await _camerasService.CheckCameraConnection(new()
            {
                IpAddress = IPAddress,
                Login = Login,
                Password = Password,
            });

            return virifyIpAddressResponse.IsSuccess && virifyIpAddressResponse.Result;
        }

        private async Task HandleContinueButtonPressedAsync()
        {
            IsIpAddressCorrect = await VerifyIpAddressCorrectAsync();

            if (!IsIpAddressCorrect)
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, Strings.NoResponseIPAddress },
                });
            }
            else
            {
                IsIpAddressEntryFocused = true;
            }
        }

        private async Task HandleAddButtonPressedAsync()
        {
            var result = await VerifyCameraConnectionAsync();

            if (result)
            {
                RequestClose.Invoke(new DialogParameters
                {
                    { Constants.DialogsParameterKeys.IP_ADDRESS, IPAddress },
                    { Constants.DialogsParameterKeys.LOGIN, Login },
                    { Constants.DialogsParameterKeys.PASSWORD, Password },
                });
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, Strings.LoginPasswordIncorrect },
                });
            }
        }

        private async Task OnAddCommandAsync()
        {
            if (!IsIpAddressCorrect)
            {
                await HandleContinueButtonPressedAsync();
            }
            else
            {
                await HandleAddButtonPressedAsync();
            }
        }

        #endregion
    }
}
