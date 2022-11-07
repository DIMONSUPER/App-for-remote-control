using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace SmartMirror;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
    Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
    DataSchemes = new[] { "smartmirror", "headworks.smartmirror", "https", "http" },
    DataHosts = new[] { "smartmirrorapp.com" })]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
}

