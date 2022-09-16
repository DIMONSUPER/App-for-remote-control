using Foundation;

namespace SmartMirror;

[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
    #region -- Overrides --

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    #endregion
}

