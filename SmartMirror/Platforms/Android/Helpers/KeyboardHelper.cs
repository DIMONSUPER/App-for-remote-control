using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using SmartMirror.Helpers;

namespace SmartMirror.Platforms.Android.Helpers
{
    public class KeyboardHelper : IKeyboardHelper
    {
        private static Context _context;

        public static void Init(Context context)
        {
            _context = context;
        }

        public void HideKeyboard()
        {
            var inputMethodManager = _context?.GetSystemService(Context.InputMethodService) as InputMethodManager;

            if (inputMethodManager is not null && _context is Activity activity)
            {
                var token = activity.CurrentFocus?.WindowToken;

                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }

        public void ShowKeyboard()
        {
            var inputMethodManager = _context?.GetSystemService(Context.InputMethodService) as InputMethodManager;

            if (inputMethodManager is not null && _context is Activity activity)
            {
                var token = activity.CurrentFocus?.WindowToken;

                inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            }
        }
    }
}
