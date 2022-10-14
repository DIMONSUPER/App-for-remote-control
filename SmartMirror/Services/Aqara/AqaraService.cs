using System.Text;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Aqara;

public class AqaraService : IAqaraService
{
    private readonly IRestService _restService;
    private readonly ISettingsManager _settingsManager;

    public AqaraService(
        IRestService restService,
        ISettingsManager settingsManager)
    {
        _restService = restService;
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

            var response = await MakeRequestAsync<BaseAqaraResponse<DataAqaraResponse<DeviceAqaraModel>>>("query.device.info", data);

            return response.Result;
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

    public Task<AOResult<DevicesResponse>> GetAllDevicesAsync()
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                pageNum = 1,
                pageSize = 100,
            };

            var response = await MakeRequestAsync<BaseAqaraResponse<DevicesResponse>>("query.device.info", data);

            if (response?.Result is null)
            {
                onFailure("response or result is null");
            }

            return response.Result;
        });
    }

    public Task<AOResult<BaseAqaraResponse>> UpdateAttributeValueAsync(string deviceId, params (string resourceId, string value)[] resources)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var array = resources.Select(x => new { resourceId = x.resourceId, value = x.value }).ToArray();

            var data = new[]
            {
                new
                {
                    subjectId = deviceId,
                    resources = array,
                }
            };

            var response = await MakeRequestAsync<BaseAqaraResponse>("write.resource.device", data);

            if (response is null)
            {
                onFailure("response is null");
            }

            return response;
        });
    }

    public Task<AOResult<IEnumerable<ResourceResponse>>> GetDeviceAttributeValueAsync(string deviceId, params string[] resourceIds)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                resources = new[]
                {
                    new
                    {
                        subjectId = deviceId,
                        resourceIds = resourceIds,
                    }
                }
            };

            var response = await MakeRequestAsync<BaseAqaraResponse<IEnumerable<ResourceResponse>>>("query.resource.value", data);

            if (response.Result is null)
            {
                onFailure("response or result is null");
            }

            return response.Result;
        });
    }

    #endregion

    #region -- Private helpers --

    private Task<T> MakeRequestAsync<T>(string intent, object data)
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

