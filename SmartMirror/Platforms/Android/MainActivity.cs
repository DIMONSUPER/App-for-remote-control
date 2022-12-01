using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.View;
using SmartMirror.ViewModels.Dialogs;
//using Com.Amazon.Identity.Auth.Device.Api.Authorization;
//using Com.Amazon.Identity.Auth.Device.Api.Workflow;

namespace SmartMirror;

[Activity(
    Theme = "@style/StartingThemeApp",
    Icon = "@mipmap/ic_launcher",
    RoundIcon = "@mipmap/ic_launcher_round",
    MainLauncher = true,
    WindowSoftInputMode = SoftInput.AdjustPan,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.UserLandscape,
    LaunchMode = LaunchMode.SingleTop,
    Exported = true)]
public class MainActivity : MauiAppCompatActivity
{
    #region -- Overrides --

    protected override void OnCreate(Bundle savedInstanceState)
    {
        AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

        base.OnCreate(savedInstanceState);
    }

    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        Window.SetFormat(Format.Rgba8888);
    }

    public override void OnBackPressed()
    {
        var modalStack = App.Current?.MainPage?.Navigation?.ModalStack;

        if (modalStack is not null && modalStack.Any())
        {
            if (modalStack[^1] is DialogContainerPage dialogContainerPage && dialogContainerPage?.DialogView?.BindingContext is BaseDialogViewModel baseDialogViewModel)
            {
                if (baseDialogViewModel.CloseCommand is not null && baseDialogViewModel.CloseCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine($"OnBackPressed {dialogContainerPage.Title}");
                    baseDialogViewModel.CloseCommand.Execute(null);
                }
            }
        }
        else
        {
            base.OnBackPressed();
        }
    }

    #endregion

    //TODO: Remove when companion app is ready
    /*private RequestContext _currentRequestContext;

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

    #endregion*/
}

