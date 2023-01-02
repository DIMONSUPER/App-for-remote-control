using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Blur;
using SmartMirror.Services.Keyboard;
using SmartMirror.Views.Dialogs;
using System.Windows.Input;
using SmartMirror.Resources.Strings;

namespace SmartMirror.ViewModels.Dialogs;

public class EnterCodeDialogViewModel : BaseDialogViewModel
{
    private readonly IAqaraService _aqaraService;
    private readonly IDialogService _dialogService;

    public EnterCodeDialogViewModel(
        IAqaraService aqaraService,
        IBlurService blurService,
        IDialogService dialogService,
        IKeyboardService keyboardService)
        : base(blurService, keyboardService)
    {
        _aqaraService = aqaraService;
        _dialogService = dialogService;
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

    public override async void OnDialogOpened(IDialogParameters parameters)
    {
        base.OnDialogOpened(parameters);

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.TITLE, out string title))
        {
            Title = title;
        }

        if (parameters.TryGetValue(Constants.DialogsParameterKeys.AUTH_TYPE, out EAuthType authType))
        {
            AuthType = authType;
        }

        await Task.Delay(FOCUS_DELAY);
    }

    #endregion

    #region -- Private helpers --

    private async Task OnContinueCommandAsync()
    {
        if (!string.IsNullOrEmpty(CodeText))
        {
            IsLoggingWithCode = true;

            var loginWithCodeResponse = await _aqaraService.LoginWithCodeAsync(Constants.Aqara.TEST_EMAIL, CodeText);

            if (loginWithCodeResponse.IsSuccess)
            {
                RequestClose.Invoke(new DialogParameters
                {
                    { Constants.DialogsParameterKeys.RESULT, true },
                });
            }
            else
            {
                IsLoggingWithCode = false;

                var errorDescription = IsInternetConnected
                    ? loginWithCodeResponse.Message
                    : Strings.ServerIsUnavailable;

                await _dialogService.ShowDialogAsync(nameof(ErrorDialog), new DialogParameters
                {
                    { Constants.DialogsParameterKeys.TITLE, "Fail!" },
                    { Constants.DialogsParameterKeys.DESCRIPTION, errorDescription }
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