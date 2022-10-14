using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;
using System.Text;
namespace SmartMirror.Services.Aqara;

public class AqaraService : IAqaraService
{
    private readonly IRestService _restService;
    private readonly IDialogService _dialogService;
    private readonly ISettingsManager _settingsManager;

    public AqaraService(
        IRestService restService,
        IDialogService dialogService,
        ISettingsManager settingsManager)
    {
        _restService = restService;
        _dialogService = dialogService;
        _settingsManager = settingsManager;
    }

    #region -- IAqaraService implementation --

    public bool IsAuthorized => !string.IsNullOrEmpty(_settingsManager.AqaraAccessSettings.AccessToken) && DateTime.UtcNow < _settingsManager.AqaraAccessSettings.ExpiresAt;

    public Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                account = email,
                accountType = 0,
                accessTokenValidity = "1d",
            };

            var response = await MakeRequestAsync<BaseAqaraResponse>("config.auth.getAuthCode", data);

            if (response is null)
            {
                onFailure("response is null");
            }

            return response;
        });
    }

    public Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                account = email,
                accountType = 0,
                authCode = code,
            };

            var response = await MakeRequestAsync<BaseAqaraResponse<AccessResponse>>("config.auth.getToken", data);

            if (response?.Result is null)
            {
                onFailure("response or result is null");
            }
            else
            {
                _settingsManager.AqaraAccessSettings.SetAccessSettings(response.Result);
            }

            return response as BaseAqaraResponse;
        });
    }

    public Task<AOResult<DataAqaraResponse<DeviceAqaraModel>>> GetDevicesByPositionAsync(string positionId, int pageNum, int pageSize)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                positionId = positionId,
                pageNum = pageNum,
                pageSize = pageSize,
            };

            var response = await MakeRequestAsync<BaseAqaraResponse<DataAqaraResponse<DeviceAqaraModel>>> ("query.device.info", data);

            if (response.Message != "Success")
            {
                onFailure("Request failed");
            }

            return response.Result;
        });
    }

    public Task<AOResult<DataAqaraResponse<PositionAqaraModel>>> GetPositionsAsync(string positionId, int pageNum, int pageSize)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var requestData = new
            {
                parentPositionId = positionId,
                pageNum = pageNum,
                pageSize = pageSize,
            };

            var response = await MakeRequestAsync<BaseAqaraResponse<DataAqaraResponse<PositionAqaraModel>>>("query.position.info", requestData);

            if (response.Message != "Success")
            {
                onFailure("Request failed");
            }

            return response.Result;
        });
    }

    public Task<AOResult<DataAqaraResponse<SimpleSceneAqaraModel>>> GetScenesAsync(int pageNumber = 1, int pageSize = 100, string positionId = null)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                positionId = positionId,
                pageNum = pageNumber,
                pageSize = pageSize,
            };

            var responce = await MakeRequestAsync<BaseAqaraResponse<DataAqaraResponse<SimpleSceneAqaraModel>>>("query.scene.listByPositionId", data);

            SetFailureIfNeed(onFailure, responce);

            return responce?.Result;
        });
    }
  
    public Task<AOResult<DetailSceneAqaraModel>> GetSceneByIdAsync(string sceneId)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                sceneId = sceneId,
            };

            var responce = await MakeRequestAsync<BaseAqaraResponse<DetailSceneAqaraModel>>("query.scene.detail", data);

            SetFailureIfNeed(onFailure, responce);

            return responce?.Result;
        });
    }

    public Task<AOResult> RunSceneByIdAsync(string sceneId)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                sceneId = sceneId,
            };

            var responce = await MakeRequestAsync<BaseAqaraResponse>("config.scene.run", data);

            SetFailureIfNeed(onFailure, responce);
        });
    }

    #endregion

    #region -- Private helpers --

    private Task<T> MakeRequestAsync<T>(string intent, object data)
    {
        var time = ((long)DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds).ToString();

        var headers = new Dictionary<string, string>
        {
            { "Appid", Constants.Aqara.APP_ID },
            { "Keyid", Constants.Aqara.KEY_ID },
            { "Nonce", time },
            { "Time", time },
            { "Sign", GetSign(time) },
        };

        if (!string.IsNullOrWhiteSpace(_settingsManager.AqaraAccessSettings.AccessToken))
        {
            headers.Add("Accesstoken", _settingsManager.AqaraAccessSettings.AccessToken);
        }

        return _restService.PostAsync<T>(Constants.Aqara.API_URL, new
        {
            intent = intent,
            data = data,
        }, headers);
    }

    private string GetSign(string time)
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(_settingsManager.AqaraAccessSettings.AccessToken))
        {
            builder.Append($"Accesstoken={_settingsManager.AqaraAccessSettings.AccessToken}&");
        }

        builder.Append($"Appid={Constants.Aqara.APP_ID}");
        builder.Append($"&Keyid={Constants.Aqara.KEY_ID}");
        builder.Append($"&Nonce={time}");
        builder.Append($"&Time={time}");
        builder.Append($"{Constants.Aqara.APP_KEY}");

        var result = builder.ToString().ToLower();

        try
        {
            System.Diagnostics.Debug.WriteLine(result);
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

    private void SetFailureIfNeed(Action<string> onFailure, BaseAqaraResponse responce)
    {
        if (responce is null)
        {
            onFailure("Response is null");
        }
        else if (responce.Message != "Success")
        {
            onFailure("Request failed");
        }
    }

    #endregion
}