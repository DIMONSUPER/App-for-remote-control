using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Google;
using SmartMirror.Services.Keyboard;
using SmartMirror.Views.Dialogs;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs
{
    public class AddMoreProviderDialogViewModel : BaseDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IGoogleService _googleService;
        private readonly INavigationService _navigationService;

        public AddMoreProviderDialogViewModel(
            IDialogService dialogService,
            IGoogleService googleService,
            IBlurService blurService,
            INavigationService navigationService,
            IKeyboardService keyboardService)
            : base(blurService, keyboardService)
        {
            _dialogService = dialogService;
            _googleService = googleService;
            _navigationService = navigationService;
        }

        #region -- Public properties --

        private EAuthType _authType;
        public EAuthType AuthType
        {
            get => _authType;
            set => SetProperty(ref _authType, value);
        }

        private bool _isFinishButtonBusy;
        public bool IsFinishButtonBusy
        {
            get => _isFinishButtonBusy;
            set => SetProperty(ref _isFinishButtonBusy, value);
        }

        private ICommand _loginWithAqaraCommand;
        public ICommand LoginWithAqaraCommand => _loginWithAqaraCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAqaraCommandAsync);

        private ICommand _loginWithAmazonCommand;
        public ICommand LoginWithAmazonCommand => _loginWithAmazonCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAmazonCommandAsync);

        private ICommand _loginWithAppleCommand;
        public ICommand LoginWithAppleCommand => _loginWithAppleCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithAppleCommandAsync);

        private ICommand _loginWithGoogleCommand;
        public ICommand LoginWithGoogleCommand => _loginWithGoogleCommand ??= SingleExecutionCommand.FromFunc(OnLoginWithGoogleCommandAsync);

        #endregion

        #region -- Overrides --

        public override async Task OnCloseCommandAsync()
        {
            IsFinishButtonBusy = true;

            await _navigationService.CreateBuilder()
                .AddSegment<MainTabbedPageViewModel>()
                .NavigateAsync();

            RequestClose.Invoke(new DialogParameters
            {
                { Constants.DialogsParameterKeys.RESULT, true },
            });

            IsFinishButtonBusy = false;
        }

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
