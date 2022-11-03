using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Resources.Strings;
using SmartMirror.Services.Aqara;
using SmartMirror.Views;
using SmartMirror.Views.Dialogs;
using System.Windows.Input;

namespace SmartMirror.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        private readonly IAqaraService _aqaraService;
        private readonly IDialogService _dialogService;

        public WelcomePageViewModel(
            IAqaraService aqaraService,
            IDialogService dialogService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _aqaraService = aqaraService;
            _dialogService = dialogService;
        }

        #region -- Public properties --

        private ICommand _loginWithAqaraCommand;
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc<EAuthType>(OnLoginWithAqaraCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnLoginWithAqaraCommandAsync(EAuthType authType)
        {
            if (authType == EAuthType.Amazon
                || authType == EAuthType.Apple
                || authType == EAuthType.Google)
            {
                DisplayNotImplementedDialog();
            }
            else if (IsInternetConnected)
            {
                var testEmail = "botheadworks@gmail.com";

                var resultOfSendingCodeToMail = await _aqaraService.SendLoginCodeAsync(testEmail);

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
                    dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                        { Constants.DialogsParameterKeys.DESCRIPTION, resultOfSendingCodeToMail.Result?.MsgDetails ?? resultOfSendingCodeToMail.Message },
                    });
                }

                await ProcessDialogResultAsync(dialogResult, testEmail);
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

        private async Task ProcessDialogResultAsync(IDialogResult dialogResult, string email)
        {
            if (dialogResult.Parameters.TryGetValue(Constants.DialogsParameterKeys.RESULT, out bool result))
            {
                await NavigationService.CreateBuilder()
                    .AddSegment<MainTabbedPage>()
                    .NavigateAsync();
            }
        }

        #endregion
    }
}
