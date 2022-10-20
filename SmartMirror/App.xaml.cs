using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace SmartMirror;

public partial class App
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnStart()
    {
        base.OnStart();

#if !DEBUG
        AppCenter.Start(
            $"android={Constants.Analytics.AndroidKey};",
            typeof(Analytics),
            typeof(Crashes));

        Analytics.SetEnabledAsync(true);
#endif
    }
}