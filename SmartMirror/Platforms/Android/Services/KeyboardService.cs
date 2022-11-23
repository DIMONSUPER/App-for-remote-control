using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Platform;
using SmartMirror.Services.Keyboard;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace SmartMirror.Platforms.Android.Services;

public class KeyboardService : IKeyboardService
{
    public KeyboardService()
    {
        SubcribeToHeightChanges();
    }

    #region -- IKeyboardService implementaion --

    public double KeyboardHeight { get; private set; }

    public event EventHandler KeyboardHeightChanged;

    #endregion

    #region -- Private helpers --

    private void SubcribeToHeightChanges()
    {
        try
        {
            var mRootView = Platform.CurrentActivity.Window?.DecorView?.FindViewById(global::Android.Resource.Id.Content);

            mRootView.ViewTreeObserver.GlobalLayout += OnGlobalLayoutChanged;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(SubcribeToHeightChanges)}: {ex.Message}");
        }
    }

    private void OnGlobalLayoutChanged(object sender, EventArgs e)
    {
        try
        {
            var rootWindow = Platform.CurrentActivity.Window;

            using var rect = new global::Android.Graphics.Rect();

            rootWindow.DecorView.GetWindowVisibleDisplayFrame(rect);

            KeyboardHeight = Platform.AppContext.FromPixels(rootWindow.DecorView.Height - rect.Bottom);

            KeyboardHeightChanged?.Invoke(this, EventArgs.Empty);

            System.Diagnostics.Debug.WriteLine($"{nameof(KeyboardHeightChanged)}: {KeyboardHeight}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(OnGlobalLayoutChanged)}: {ex.Message}");
        }

    }

    #endregion
}

