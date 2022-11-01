using SmartMirror.Services.Blur;

namespace SmartMirror.ViewModels.Dialogs;

public class BaseDialogViewModel : BindableBase, IDialogAware
{
    public BaseDialogViewModel(IBlurService blurService)
    {
        BlurService = blurService;
        BlurService.BlurPopupBackground();
    }

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

