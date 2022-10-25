using SmartMirror.Helpers;
using SmartMirror.Services.Aqara;
using SmartMirror.ViewModels.Dialogs;
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
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAqaraCommandAsync);

        private Stream _screenStream;
        public Stream ScreenStream
        {
            get => _screenStream;
            set => SetProperty(ref _screenStream, value);
        }

        #endregion

        #region -- Private helpers --

        private async Task OnLoginWithAqaraCommandAsync()
        {
            if (IsInternetConnected)
            {
                var testEmail = "botheadworks@gmail.com";

                //var resultOfSendingCodeToMail = await _aqaraService.SendLoginCodeAsync(testEmail);

                IDialogResult dialogResult;

                if (true)
                //if (resultOfSendingCodeToMail.IsSuccess)
                {
                    dialogResult = await _dialogService.ShowDialogAsync(nameof(TemporaryDialog), new DialogParameters
                    {
                        { "ScreenStream", ScreenStream }
                    });
                }
                else
                {
                    //dialogResult = await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    //{
                    //    { Constants.DialogsParameterKeys.TITLE, "FAIL" },
                    //    { Constants.DialogsParameterKeys.DESCRIPTION, resultOfSendingCodeToMail.Result?.MsgDetails ?? resultOfSendingCodeToMail.Message },
                    //});
                }

                await ProcessDialogResultAsync(dialogResult, testEmail);
            }
            else
            {
                //TODO: notify
            }
        }

        private async Task ProcessDialogResultAsync(IDialogResult dialogResult, string email)
        {
            if (dialogResult.Parameters.TryGetValue(nameof(TemporaryDialogViewModel.CodeText), out string code))
            {
                var loginWithCodeResponse = await _aqaraService.LoginWithCodeAsync(email, code);

                if (loginWithCodeResponse.IsSuccess)
                {
                    await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "Success!" }
                    });

                    await NavigationService.CreateBuilder()
                        .AddSegment<MainTabbedPage>()
                        .NavigateAsync();
                }
                else
                {
                    await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                    {
                        { Constants.DialogsParameterKeys.TITLE, "Fail!" },
                        { Constants.DialogsParameterKeys.DESCRIPTION, loginWithCodeResponse.Message }
                    });
                }
            }
        }

        #endregion
    }
}
