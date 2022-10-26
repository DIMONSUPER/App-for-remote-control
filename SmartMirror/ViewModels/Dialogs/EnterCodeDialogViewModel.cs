using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Views.Dialogs;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs;

public class EnterCodeDialogViewModel : BaseDialogViewModel
{
    private readonly IAqaraService _aqaraService;
    private readonly IDialogService _dialogService;
    private readonly IKeyboardService _keyboardService;

    public EnterCodeDialogViewModel(
        IAqaraService aqaraService,
        IKeyboardService keyboardService,
        IBlurService blurService,
        IDialogService dialogService)
        : base(blurService)
    {
        _aqaraService = aqaraService;
        _dialogService = dialogService;
        _keyboardService = keyboardService;
    }

    #region -- Public properties --

    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _codeText;
    public string CodeText
    {
        get => _codeText;
        set => SetProperty(ref _codeText, value);
    }

    private bool _isLoggingWithCode;
    public bool IsLoggingWithCode
    {
        get => _isLoggingWithCode;
        set => SetProperty(ref _isLoggingWithCode, value);
    }

    private EAuthType _authType;
    public EAuthType AuthType
    {
        get => _authType;
        set => SetProperty(ref _authType, value);
    }

    private ICommand _continueCommand;
    public ICommand ContinueCommand => _continueCommand ??= SingleExecutionCommand.FromFunc(OnContinueCommandAsync);

    #endregion

    #region -- Overrides --

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        _keyboardService.ShowKeyboard();

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
        {
            Title = title;
        }

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.AUTH_TYPE, out EAuthType authType))
        {
            AuthType = authType;
        }
    }

    #endregion

    #region -- Private helpers --

    private async Task OnContinueCommandAsync()
    {
        if (!string.IsNullOrEmpty(CodeText))
        {
            IsLoggingWithCode = true;

            var testEmail = "botheadworks@gmail.com";

            var loginWithCodeResponse = await _aqaraService.LoginWithCodeAsync(testEmail, CodeText);

            if (loginWithCodeResponse.IsSuccess)
            {
                _keyboardService.HideKeyboard();

                RequestClose.Invoke(new DialogParameters
                {
                    { Constants.DialogsParameterKeys.RESULT, true },
                });
            }
            else
            {
                IsLoggingWithCode = false;

                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Fail!" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, loginWithCodeResponse.Message }
                });
            }
        }
        else
        {
            //ToDo: no state
        }
    }

    #endregion
}

