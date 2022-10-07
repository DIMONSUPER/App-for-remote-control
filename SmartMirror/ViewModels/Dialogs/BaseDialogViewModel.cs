using System;
namespace SmartMirror.ViewModels.Dialogs
{
    public class BaseDialogViewModel : BindableBase, IDialogAware
    {
        public BaseDialogViewModel()
        {
        }

        #region -- IDialogAware implementation --

        public DialogCloseEvent RequestClose { get; set; }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #endregion
    }
}

