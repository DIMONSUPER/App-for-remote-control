using System.Text;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Aqara
{
    public abstract class BaseAqaraService
    {
        public BaseAqaraService(
            IRestService restService,
            ISettingsManager settingsManager)
        {
            RestService = restService;
            SettingsManager = settingsManager;
        }

        #region -- Protected properties --

        protected IRestService RestService { get; }

        protected ISettingsManager SettingsManager { get; }

        #endregion

        #region -- Protected helpers --

        protected async Task<BaseAqaraResponse<T>> MakeRequestAsync<T>(string intent, object data, Action<string> onFailure) where T : class
        {
            var result = await MakeAqaraPostAsync<BaseAqaraResponse<T>>(intent, data);

            if (result is null)
            {
                onFailure("result is null");
            }
            else if (result.Code != 0)
            {
                System.Diagnostics.Debug.WriteLine($"{result.Code}: {result.Message} {result.MsgDetails}");
                onFailure($"{result.Code}: {result.Message} {result.MsgDetails}");
            }
            else if (result.Result is null)
            {
                onFailure("Returned result is null");
            }

            return result;
        }

        protected async Task<BaseAqaraResponse> MakeRequestAsync(string intent, object data, Action<string> onFailure)
        {
            var result = await MakeAqaraPostAsync<BaseAqaraResponse>(intent, data);

            if (result is null)
            {
                onFailure("result is null");
            }
            else if (result.Code != 0)
            {
                System.Diagnostics.Debug.WriteLine($"{result.Code}: {result.Message}, {result.MsgDetails}");
                onFailure($"{result.Code}: {result.Message}, {result.MsgDetails}");
            }

            return result;
        }

        #endregion

        #region -- Private helpers --

        private async Task<T> MakeAqaraPostAsync<T>(string intent, object data)
        {
            if (SettingsManager.AqaraAccessSettings.ExpiresAt < DateTime.UtcNow && !string.IsNullOrWhiteSpace(SettingsManager.AqaraAccessSettings.RefreshToken))
            {
                await RefreshAndSetTokenAsync();
            }

            return await RestService.PostAsync<T>(Constants.Aqara.API_URL, new
            {
                intent = intent,
                data = data,
            }, GetHeaders());
        }

        private Task<AOResult<AccessResponse>> RefreshTokenAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var data = new
                {
                    refreshToken = SettingsManager.AqaraAccessSettings.RefreshToken,
                };

                var intent = "config.auth.refreshToken";

                var result = await RestService.PostAsync<BaseAqaraResponse<AccessResponse>>(Constants.Aqara.API_URL, new
                {
                    intent = intent,
                    data = data,
                }, GetHeaders());

                if (result?.Result is null)
                {
                    onFailure($"{result?.Code}: {result?.Message}, {result?.MsgDetails}");
                }

                return result?.Result;
            });
        }

        private async Task RefreshAndSetTokenAsync()
        {
            //Token has expired
            var refreshResponse = await RefreshTokenAsync();

            if (refreshResponse.IsSuccess)
            {
                SettingsManager.AqaraAccessSettings.SetAccessSettings(refreshResponse.Result);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Can't refresh token {refreshResponse.Message}");
                SettingsManager.AqaraAccessSettings.Clear();
            }
        }

        private Dictionary<string, string> GetHeaders()
        {
            var time = DateTimeHelper.ConvertToMilliseconds(DateTime.UtcNow).ToString();

            var headers = new Dictionary<string, string>
            {
                { "Appid", Constants.Aqara.APP_ID },
                { "Keyid", Constants.Aqara.KEY_ID },
                { "Nonce", time },
                { "Time", time },
                { "Sign", GetSign(time) },
            };

            if (!string.IsNullOrWhiteSpace(SettingsManager.AqaraAccessSettings.AccessToken))
            {
                headers.Add("Accesstoken", SettingsManager.AqaraAccessSettings.AccessToken);
            }

            return headers;
        }

        private string GetSign(string time)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(SettingsManager.AqaraAccessSettings.AccessToken))
            {
                builder.Append($"Accesstoken={SettingsManager.AqaraAccessSettings.AccessToken}&");
            }

            builder.Append($"Appid={Constants.Aqara.APP_ID}");
            builder.Append($"&Keyid={Constants.Aqara.KEY_ID}");
            builder.Append($"&Nonce={time}");
            builder.Append($"&Time={time}");
            builder.Append($"{Constants.Aqara.APP_KEY}");

            var result = builder.ToString().ToLower();

            try
            {
                return GetMD5FromString(result).ToLower();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private string GetMD5FromString(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }

        #endregion
    }
}

