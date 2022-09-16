using Android.App;
using Android.Runtime;

namespace SmartMirror;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(
		IntPtr handle,
		JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

    #region -- Overrides --

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    #endregion
}

