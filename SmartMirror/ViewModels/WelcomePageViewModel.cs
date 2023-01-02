using CommunityToolkit.Maui.Alerts;
using Prism.Services;
using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Google;
using SmartMirror.Views.Dialogs;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        private readonly IAqaraService _aqaraService;
        private readonly IDialogService _dialogService;
        private readonly IGoogleService _googleService;
        private int _buttonCount;

        public WelcomePageViewModel(
            IAqaraService aqaraService,
            IDialogService dialogService,
            INavigationService navigationService,
            IGoogleService googleService)
            : base(navigationService)
        {
            _aqaraService = aqaraService;
            _dialogService = dialogService;
            _googleService = googleService;
        }

        #region -- Public properties --

        private ICommand _loginWithAqaraCommand;
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc<EAuthType>(OnLoginWithAqaraCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

        private ICommand _loginWithGoogleCommand;
        public ICommand LoginWithGoogleCommand => _loginWithGoogleCommand ??= SingleExecutionCommand.FromFunc<EAuthType>(OnLoginWithGoogleCommandAsync, true, Constants.Limits.DELAY_MILLISEC_NAVIGATION_COMMAND);

        #endregion

        #region -- Overrides --

        public override bool OnBackButtonPressed()
        {
            if (_buttonCount < 1)
            {
                var interval = TimeSpan.FromMilliseconds(500);
                Application.Current.Dispatcher.StartTimer(interval, GetCountBackButtonPresses);
            }

            _buttonCount++;

            return true;
        }

        #endregion

        #region -- Private helpers --

        private bool GetCountBackButtonPresses()
        {
            if (_buttonCount > 1)
            {
                Application.Current.Quit();
            }
            else
            {
                Toast.Make(Strings.NeedsTwoTaps).Show();
            }

            _buttonCount = 0;

            return false;
        }

        private async Task OnLoginWithGoogleCommandAsync(EAuthType authType)
        {
            var result = await _googleService.AutorizeAsync();

            if (result.IsSuccess)
            {
                //TODO: implement when have nest devices
            }
            else
            {
                //TODO: implement if needed
            }
        }

        private async Task OnLoginWithAqaraCommandAsync(EAuthType authType)
        {
            if (authType == EAuthType.Amazon || authType == EAuthType.Apple)
            {
                DisplayNotImplementedDialog();
            }
            else if (IsInternetConnected)
            {
                var resultOfSendingCodeToMail = await _aqaraService.SendLoginCodeAsync(Constants.Aqara.TEST_EMAIL);

                IDialogResult dialogResult;

                if (resultOfSendingCodeToMail.IsSuccess)
                {
                    dialogResult = await _dialogService.ShowDialogAsync(nameof(EnterCodeDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, Strings.Aqara },
                        { Constants.DialogsParameterKeys.AUTH_TYPE, authType },
                    });
                }
                else
                {
                    var errorDescription = IsInternetConnected
                        ? resultOfSendingCodeToMail.Result?.MsgDetails ?? resultOfSendingCodeToMail.Message
                        : Strings.ServerIsUnavailable;

                    dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                        { Constants.DialogsParameterKeys.DESCRIPTION, errorDescription },
                    });
                }

                await ProcessDialogResultAsync(dialogResult, authType);
            }
            else
            {
                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, Strings.NoInternetConnection },
                    { Constants.DialogsParameterKeys.DESCRIPTION, Strings.PleaseCheckInternet },
                });
            }
        }

        private void DisplayNotImplementedDialog()
        {
            _dialogService.ShowDialog(nameof(ErrorDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, "It's not available right now" },
                { Constants.DialogsParameterKeys.DESCRIPTION, $"Coming soon" },
            });
        }

        private async Task ProcessDialogResultAsync(IDialogResult response, EAuthType authType)
        {
            if (response.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool resultAuth) && resultAuth)
            {
                await _dialogService.ShowDialogAsync(nameof(AddMoreProviderDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.AUTH_TYPE, authType },
                });
            }
        }

        #endregion
    }
}