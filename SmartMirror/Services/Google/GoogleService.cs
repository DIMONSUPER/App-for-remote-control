using SmartMirror.Helpers;

namespace SmartMirror.Services.Google;

public class GoogleService : IGoogleService
{
    public GoogleService()
    {
    }

    #region -- IGoogleService implementation --

    public Task<AOResult> AutorizeAsync()
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var link = $"https://nestservices.google.com/partnerconnections/" +
                $"{Constants.Google.PROJECT_ID}/auth?" +
                $"redirect_uri={Constants.Google.REDIRECT_URI}" +
                $"&client_id={Constants.Google.WEB_CLIENT_ID}" +
                $"&access_type=offline" +
                $"&prompt=consent" +
                $"&response_type=code" +
                $"&scope={Constants.Google.SDM_SERVICE_SCOPE}";

            var url = new Uri(link);

            var callbackUrl = new Uri("https://smartmirrorapp.com");

            var authResult = await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions
            {
                Url = url,
                CallbackUrl = callbackUrl,
                PrefersEphemeralWebBrowserSession = true,
            });
        });
    }

    #endregion
}

