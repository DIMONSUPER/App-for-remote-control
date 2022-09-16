using Android.App;
using Android.Content.PM;

namespace SmartMirror;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.UserLandscape)]
public class MainActivity : MauiAppCompatActivity
{
}

