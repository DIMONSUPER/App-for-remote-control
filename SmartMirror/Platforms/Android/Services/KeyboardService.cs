using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using SmartMirror.Services.Keyboard;

namespace SmartMirror.Platforms.Android.Services
{
    public class KeyboardService : IKeyboardService
    {
        #region -- IKeyboardService implementation --

        public void HideKeyboard()
        {
            var inputMethodManager = Platform.CurrentActivity.GetSystemService(Context.InputMethodService) as InputMethodManager;

            if (inputMethodManager is not null && Platform.CurrentActivity is Activity activity)
            {
                var token = activity.CurrentFocus?.WindowToken;

                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }

        public void ShowKeyboard()
        {
            var inputMethodManager = Platform.CurrentActivity.GetSystemService(Context.InputMethodService) as InputMethodManager;

            if (inputMethodManager is not null && Platform.CurrentActivity is Activity activity)
            {
                var token = activity.CurrentFocus?.WindowToken;

                inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            }
        }

        #endregion
    }
}
