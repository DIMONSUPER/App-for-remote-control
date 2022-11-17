using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Google;
using SmartMirror.Views.Dialogs;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class AddMoreProviderDialogViewModel : BaseDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IGoogleService _googleService;

        public AddMoreProviderDialogViewModel(
            IDialogService dialogService,
            IGoogleService googleService,
            IBlurService blurService)
            : base(blurService)
        {
            _dialogService = dialogService;
            _googleService = googleService;
        }

        #region -- Public properties --

        private EAuthType _authType;
        public EAuthType AuthType
        {
            get => _authType;
            set => SetProperty(ref _authType, value);
        }

        private ICommand _loginWithAqaraCommand;
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAqaraCommandAsync);

        private ICommand _loginWithAmazonCommand;
        public ICommand LoginWithAmazonCommand => _loginWithAmazonCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAmazonCommandAsync);

        private ICommand _loginWithAppleCommand;
        public ICommand LoginWithAppleCommand => _loginWithAppleCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAppleCommandAsync);

        private ICommand _loginWithGoogleCommand;
        public ICommand LoginWithGoogleCommand => _loginWithGoogleCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithGoogleCommandAsync);

        private ICommand _finishCommand;
        public ICommand FinishCommand => _finishCommand ??= SingleExecutionCommand.FromFunc(OnFinishCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Constants.DialogsParameterKeys.AUTH_TYPE, out EAuthType authType))
            {
                AuthType = authType;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnLoginWithAqaraCommandAsync()
        {
            return DisplayNotImplementedDialogAsync();
        }

        private Task OnLoginWithAmazonCommandAsync()
        {
            return DisplayNotImplementedDialogAsync();
        }

        private Task OnLoginWithAppleCommandAsync()
        {
            return DisplayNotImplementedDialogAsync();
        }

        private async Task OnLoginWithGoogleCommandAsync()
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

        private Task OnFinishCommandAsync()
        {
            RequestClose.Invoke(new DialogParameters
            {
                { Constants.DialogsParameterKeys.RESULT, true },
            });

            return Task.CompletedTask;
        }

        private Task DisplayNotImplementedDialogAsync()
        {
            return _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
            {
                { Constants.DialogsParameterKeys.TITLE, "It's not available right now" },
                { Constants.DialogsParameterKeys.DESCRIPTION, $"Coming soon" },
            });
        }

        #endregion
    }
}
