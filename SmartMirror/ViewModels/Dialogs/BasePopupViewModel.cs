using System;
using SmartMirror.Services.Blur;

namespace SmartMirror.ViewModels.Dialogs
{
    public class BasePopupViewModel : BindableBase
    {
        public BasePopupViewModel(IBlurService blurService)
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

        #region -- Public helpers --

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #endregion
    }
}

