using SmartMirror.Helpers;
using System.Windows.Input;

namespace SmartMirror.ViewModels.Dialogs;

public class TemporaryDialogViewModel : BaseDialogViewModel
{
    public TemporaryDialogViewModel()
    {
    }

    #region -- Public properties --

    private ICommand _closeCommand;
    public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

    private string _codeText;
    public string CodeText
    {
        get => _codeText;
        set => SetProperty(ref _codeText, value);
    }

    #endregion

    #region -- Private helpers --

    private Task OnCloseCommandAsync()
    {
        RequestClose.Invoke(new DialogParameters
        {
            { nameof(CodeText), CodeText },
        });

        return Task.CompletedTask;
    }

    #endregion
}

