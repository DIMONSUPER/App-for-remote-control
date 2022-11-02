using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Aqara;

public class AqaraService : BaseAqaraService, IAqaraService
{
    public AqaraService(
        IRestService restService,
        ISettingsManager settingsManager)
        : base(restService, settingsManager)
    {
    }

    #region -- IAqaraService implementation --

    public bool IsAuthorized => !string.IsNullOrEmpty(SettingsManager.AqaraAccessSettings.AccessToken);

    public Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                account = email,
                accountType = 0,
                accessTokenValidity = "30d",
            };

            var response = await MakeRequestAsync("config.auth.getAuthCode", data, onFailure);

            return response;
        });
    }

    public Task<AOResult<DataAqaraResponse<PositionAqaraModel>>> GetPositionsAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var requestData = new
            {
                parentPositionId = positionId,
                pageNum = pageNum,
                pageSize = pageSize,
            };

            var response = await MakeRequestAsync<DataAqaraResponse<PositionAqaraModel>>("query.position.info", requestData, onFailure);

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

            var response = await MakeRequestAsync<AccessResponse>("config.auth.getToken", data, onFailure);

            if (response?.Result is not null)
            {
                SettingsManager.AqaraAccessSettings.SetAccessSettings(response?.Result);
            }

            return response as BaseAqaraResponse;
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

            var response = await MakeRequestAsync<DataAqaraResponse<SimpleSceneAqaraModel>>("query.scene.listByPositionId", data, onFailure);

            return response?.Result;
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

            var response = await MakeRequestAsync<DetailSceneAqaraModel>("query.scene.detail", data, onFailure);

            return response?.Result;
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

            var response = await MakeRequestAsync<BaseAqaraResponse>("config.scene.run", data, onFailure);
        });
    }

    public Task<AOResult> LogoutFromAqara()
    {
        return AOResult.ExecuteTaskAsync(onFailure =>
        {
            //TODO implement when is needed
            //SettingsManager.AqaraAccessSettings.Clear();

            return Task.CompletedTask;
        });
    }

    #endregion
}