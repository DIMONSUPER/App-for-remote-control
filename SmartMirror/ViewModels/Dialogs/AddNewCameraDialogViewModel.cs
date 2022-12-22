using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Cameras;
using System.Windows.Input;
using SmartMirror.Views.Dialogs;
using Prism.Services;
using SmartMirror.Enums;
using SmartMirror.Services.Keyboard;

namespace SmartMirror.ViewModels.Dialogs
{
    public class AddNewCameraDialogViewModel : BaseDialogViewModel
    {
        private const int TIMEOUT_SECONDS = 30;

        private readonly IDialogService _dialogService;
        private readonly ICamerasService _camerasService;

        private CancellationTokenSource _verifyingCancellationTokenSource;

        public AddNewCameraDialogViewModel(
            IBlurService blurService,
            IDialogService dialogService,
            ICamerasService camerasService,
            IKeyboardService keyboardService)
            : base(blurService, keyboardService)
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

        private string _cameraName;
        public string CameraName
        {
            get => _cameraName;
            set => SetProperty(ref _cameraName, value);
        }

        private string _ipAddress;
        public string IPAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        private bool _isButtonBusy;
        public bool IsButtonBusy
        {
            get => _isButtonBusy;
            set => SetProperty(ref _isButtonBusy, value);
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

        private ICommand _addCameraCommand;
        public ICommand AddCameraCommand => _addCameraCommand ??= SingleExecutionCommand.FromFunc(OnAddCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
            {
                Title = title;

                await Task.Delay(FOCUS_DELAY);
            }
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();

            _verifyingCancellationTokenSource?.Cancel();
            _verifyingCancellationTokenSource?.Dispose();
        }

        #endregion

        #region -- Private helpers --

        private async Task ShowTooLongPopupAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(TIMEOUT_SECONDS), _verifyingCancellationTokenSource.Token).ConfigureAwait(false);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (!_verifyingCancellationTokenSource.IsCancellationRequested)
                {
                    var dialogParameters = new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, Strings.RequestTakesTooMuchTime },
                        { Constants.DialogsParameterKeys.DESCRIPTION, Strings.YouCanCancelTheRequest },
                        { Constants.DialogsParameterKeys.CONFIRM_TEXT, Strings.Wait },
                        { Constants.DialogsParameterKeys.CANCEL_TEXT, Strings.Cancel },
                    };

                    var dialogResult = await _dialogService.ShowDialogAsync(nameof(ConfirmDialog), dialogParameters);

                    if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result) && result)
                    {
                        _ = ShowTooLongPopupAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        _verifyingCancellationTokenSource?.Cancel();
                    }
                }
            });
        }

        private async Task<EResultStatus> VerifyIpAddressCorrectAsync()
        {
            EResultStatus result;

            _verifyingCancellationTokenSource = new();

            _ = ShowTooLongPopupAsync().ConfigureAwait(false);

            var virifyIpAddressResponse = await _camerasService.VerifyCameraIPAddressAsync(IPAddress, _verifyingCancellationTokenSource.Token);

            if (_verifyingCancellationTokenSource.IsCancellationRequested)
            {
                result = EResultStatus.Canceled;
            }
            else
            {
                result = (virifyIpAddressResponse.IsSuccess && virifyIpAddressResponse.Result)
                    ? EResultStatus.Success
                    : EResultStatus.Failed;

                _verifyingCancellationTokenSource.Cancel();
            }

            return result;
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
            var verifyResult = await VerifyIpAddressCorrectAsync();

            IsIpAddressCorrect = verifyResult is EResultStatus.Success;

            if (verifyResult is EResultStatus.Failed)
            {
                var errorDescription = IsInternetConnected
                    ? Strings.NoResponseIPAddress
                    : Strings.ServerIsUnavailable;

                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, errorDescription },
                });
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
                    { Constants.DialogsParameterKeys.NAME, CameraName },
                    { Constants.DialogsParameterKeys.LOGIN, Login },
                    { Constants.DialogsParameterKeys.PASSWORD, Password },
                });
            }
            else
            {
                var errorDescription = IsInternetConnected
                    ? Strings.LoginPasswordIncorrect
                    : Strings.ServerIsUnavailable;

                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, errorDescription },
                });
            }
        }

        private async Task OnAddCommandAsync()
        {
            IsButtonBusy = true;

            if (!IsIpAddressCorrect)
            {
                await HandleContinueButtonPressedAsync();
            }
            else
            {
                await HandleAddButtonPressedAsync();
            }

            IsButtonBusy = false;
        }

        #endregion
    }
}
