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


        #endregion

        #region -- Private helpers --

        protected async Task<BaseAqaraResponse<T>> MakeRequestAsync<T>(string intent, object data, Action<string> onFailure)
        {
            var result = await RestService.PostAsync<BaseAqaraResponse<T>>(Constants.Aqara.API_URL, new
            {
                intent = intent,
                data = data,
            }, GetHeaders());

            if (result is null)
            {
                onFailure("result is null");
            }
            else if (result.Code != 0)
            {
                onFailure($"{result.Code}: {result.Message}, {result.MsgDetails}");
            }

            return result;
        }

        protected async Task<BaseAqaraResponse> MakeRequestAsync(string intent, object data, Action<string> onFailure)
        {
            var result = await RestService.PostAsync<BaseAqaraResponse>(Constants.Aqara.API_URL, new
            {
                intent = intent,
                data = data,
            }, GetHeaders());

            if (result is null)
            {
                onFailure("result is null");
            }
            else if (result.Code != 0)
            {
                onFailure($"{result.Code}: {result.Message}, {result.MsgDetails}");
            }

            return result;
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

