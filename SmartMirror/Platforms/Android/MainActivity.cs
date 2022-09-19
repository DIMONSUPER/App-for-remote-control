using Android.App;
using Android.Content;
using Android.Content.PM;
using Com.Amazon.Identity.Auth.Device.Api.Authorization;
using Com.Amazon.Identity.Auth.Device.Api.Workflow;
using SmartMirror.Platforms.Services;

namespace SmartMirror;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.UserLandscape,
    Exported = true)]
[IntentFilter(
    new[] { Intent.ActionMain },
    Categories = new string[] { Intent.CategoryLauncher })]
public class MainActivity : MauiAppCompatActivity
{
    private RequestContext _currentRequestContext;
    private CustomAuthorizeListener _currentListener;

    #region -- Overrides --

    protected override void OnResume()
    {
        base.OnResume();

        _currentRequestContext?.OnResume();
    }

    #endregion

    #region -- Public helpers --

    public void StartAmazonAuthorization(EventHandler<AuthorizeResult> eventHandler)
    {
        _currentListener ??= new CustomAuthorizeListener(eventHandler);

        if (_currentRequestContext is null)
        {
            _currentRequestContext = RequestContext.Create(this);
            _currentRequestContext.RegisterListener(_currentListener);
        }

        AuthorizationManager.Authorize(new AuthorizeRequest.Builder(_currentRequestContext)
                        .AddScopes(ProfileScope.Profile(), ProfileScope.PostalCode())
                        .Build());
    }

    #endregion
}

