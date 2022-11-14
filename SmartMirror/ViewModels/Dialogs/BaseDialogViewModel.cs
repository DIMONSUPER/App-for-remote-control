using SmartMirror.Services.Blur;

namespace SmartMirror.ViewModels.Dialogs;

public class BaseDialogViewModel : BindableBase, IDialogAware
{
    public BaseDialogViewModel(IBlurService blurService)
    {
        BlurService = blurService;

        var blurColor = Color.FromArgb("#80030303");

        BlurService.BlurPopupBackground(blurColor);
    }

    #region -- Protected properties --

    protected bool IsInternetConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    #endregion

    #region -- Protected properties --

    IBlurService BlurService { get; }

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
}

