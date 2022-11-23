using System.Windows.Input;
using SmartMirror.Helpers;
using SmartMirror.Services.Blur;

namespace SmartMirror.ViewModels.Dialogs;

public class BaseDialogViewModel : BindableBase, IDialogAware
{
    protected const int FOCUS_DELAY = 350;

    public BaseDialogViewModel(IBlurService blurService)
    {
        BlurService = blurService;

        var blurColor = Color.FromArgb("#80030303");

        BlurService.BlurPopupBackground(blurColor);
    }

    #region -- Protected properties --

    protected bool IsInternetConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    IBlurService BlurService { get; }

    private ICommand _closeCommand;
    public ICommand CloseCommand => _closeCommand ??= SingleExecutionCommand.FromFunc(OnCloseCommandAsync);

    #endregion

    #region -- IDialogAware implementation --

    public DialogCloseEvent RequestClose { get; set; }

    public virtual bool CanCloseDialog()
    {
        return true;
    }

    public virtual void OnDialogClosed()
    {
        BlurService.UnblurPopupBackground();
    }

    public virtual void OnDialogOpened(IDialogParameters parameters)
    {
    }

    #endregion

    #region -- Public helpers --

    public virtual Task OnCloseCommandAsync() => Task.CompletedTask;

    #endregion
}