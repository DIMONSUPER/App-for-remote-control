using Android.App;
using Android.Content;
using Android.Content.PM;
using Com.Amazon.Identity.Auth.Device.Api.Authorization;
using Com.Amazon.Identity.Auth.Device.Api.Workflow;
using Org.Json;

namespace SmartMirror;

[Activity(
    Theme = "@style/MainThemeApp",
    Icon = "@mipmap/appicon",
    RoundIcon = "@mipmap/appicon_round",
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

    #region -- Overrides --

    protected override void OnResume()
    {
        base.OnResume();

        _currentRequestContext?.OnResume();
    }

    #endregion

    #region -- Public helpers --

    public void StartAmazonAuthorization(AuthorizeListener authorizeListener, string codeChallenge)
    {
        if (_currentRequestContext is null)
        {
            _currentRequestContext = RequestContext.Create(this);
            _currentRequestContext.RegisterListener(authorizeListener);
        }

        var scopeData = new JSONObject();
        var productInstanceAttributes = new JSONObject();

        productInstanceAttributes.Put("deviceSerialNumber", Constants.Amazon.PRODUCT_DSN);
        scopeData.Put("productInstanceAttributes", productInstanceAttributes);
        scopeData.Put("productID", Constants.Amazon.PRODUCT_ID);

        AuthorizationManager.Authorize(new AuthorizeRequest.Builder(_currentRequestContext)
            .AddScopes(ScopeFactory.ScopeNamed("alexa:voice_service:pre_auth"), ScopeFactory.ScopeNamed("alexa:all", scopeData))
            .AddScopes(ProfileScope.Profile())
            .ForGrantType(AuthorizeRequest.GrantType.AuthorizationCode)
            .WithProofKeyParameters(codeChallenge, "S256")
            .Build());
    }

    #endregion
}

