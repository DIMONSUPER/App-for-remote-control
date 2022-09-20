using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Com.Amazon.Identity.Auth.Device;
using Com.Amazon.Identity.Auth.Device.Api.Authorization;
using Com.Amazon.Identity.Auth.Device.Api.Workflow;
using SmartMirror.Helpers;
using SmartMirror.Models.Amazon;
using SmartMirror.Services.Amazon;
using SmartMirror.Services.Rest;

namespace SmartMirror.Platforms.Services
{
    public class AmazonService : IAmazonService
    {
        private readonly IRestService _restService;
        private const string TOKEN_URL = $"{Constants.Amazon.API_URL}/auth/o2/token";

        private AuthorizeListener _currentListener;

        public AmazonService(IRestService restService)
        {
            _restService = restService;
        }

        private string _codeVerifier;
        private string CodeVerifier => _codeVerifier ??= GenerateCodeVerifier();

        #region -- IAmazonService implementation --

        public event EventHandler<AuthorizeResult> AuthorizationFinished;

        public Task<AOResult> StartAuthorizationAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                if (Platform.CurrentActivity is MainActivity mainActivity)
                {
                    _currentListener ??= new CustomAuthorizeListener(AuthorizationFinished);
                    mainActivity.StartAmazonAuthorization(_currentListener, GenerateCodeChallenge(CodeVerifier));
                }
                else
                {
                    onFailure("Current activity is not main activity");
                }

                return Task.CompletedTask;
            });
        }

        public Task<AOResult<AuthResponse>> AuthorizeAsync(AuthorizeResult authorizeResult)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var parameters = new Dictionary<string, string>
                {
                    { "grant_type","authorization_code" },
                    { "code", authorizeResult.AuthorizationCode },
                    { "client_id", authorizeResult.ClientId },
                    { "redirect_uri", authorizeResult.RedirectURI },
                    { "code_verifier",  CodeVerifier }
                };

                var response = await _restService.PostAsync<AuthResponse>(TOKEN_URL, parameters);

                if (response is null)
                {
                    onFailure("response is null");
                }

                return response;
            });
        }

        #endregion

        #region -- Private helpers --

        private string GenerateCodeVerifier()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var random = new Random();
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
            {
                nonce[i] = chars[random.Next(chars.Length)];
            }

            return new string(nonce);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }

        #endregion
    }

    internal class CustomAuthorizeListener : AuthorizeListener
    {
        private event EventHandler<AuthorizeResult> _currentEventHandler;

        public CustomAuthorizeListener(EventHandler<AuthorizeResult> eventHandler)
        {
            _currentEventHandler = eventHandler;
        }

        #region -- AuthorizeListener implementation --

        public override void OnCancel(AuthCancellation authCancellation)
        {
            _currentEventHandler?.Invoke(this, null);
        }

        public override void OnError(AuthError authError)
        {
            _currentEventHandler?.Invoke(this, null);
        }

        public override void OnSuccess(AuthorizeResult authorizeResult)
        {
            _currentEventHandler?.Invoke(this, authorizeResult);
        }

        #endregion
    }
}

