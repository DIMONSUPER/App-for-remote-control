using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.Dahua;
using SmartMirror.Models.DTO;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Repository;
using SmartMirror.Services.Rest;

namespace SmartMirror.Services.Cameras
{
    public class CamerasService : ICamerasService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IMapperService _mapperService;
        private readonly IRestService _restService;

        private int _dahuaRequestId;
        private string _dahuaSessionId;
        private string _dahuaHost;

        public CamerasService(
            IRepositoryService repositoryService,
            IMapperService mapperService,
            IRestService restService)
        {
            _repositoryService = repositoryService;
            _mapperService = mapperService;
            _restService = restService;
        }

        #region -- ICamerasService implementation --

        public event EventHandler AllCamerasChanged;

        public Task<AOResult<bool>> VerifyCameraIPAddressAsync(string ipAddress, CancellationToken? cancellationToken = null)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5),
                };

                var response = await httpClient.GetAsync($"http://{ipAddress}", cancellationToken ?? CancellationToken.None);

                return response?.StatusCode is System.Net.HttpStatusCode.OK;
            });
        }

        public Task<AOResult<bool>> CheckCameraConnection(CameraBindableModel camera, CancellationToken? cancellationToken = null)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var url = new Uri($"rtsp://{camera.Login}:{camera.Password}@{camera.IpAddress}/live");

                using var rtspClient = new RtspClientSharp.RtspClient(new RtspClientSharp.ConnectionParameters(url));

                try
                {
                    await rtspClient.ConnectAsync(cancellationToken ?? CancellationToken.None);
                }
                catch (OperationCanceledException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"{nameof(CheckCameraConnection)}: {ex.Message}");
                }

                return true;
            });
        }

        public Task<AOResult<IEnumerable<CameraBindableModel>>> GetCamerasAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var cameras = Enumerable.Empty<CameraBindableModel>();

                var camerasDb = await _repositoryService.GetAllAsync<CameraDTO>();

                cameras = _mapperService.MapRange<CameraBindableModel>(camerasDb);

                return cameras;
            });
        }

        public Task<AOResult> UpdateCameraAsync(CameraBindableModel cameraModel)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var updateCamera = _mapperService.Map<CameraDTO>(cameraModel);

                var response = await _repositoryService.SaveOrUpdateAsync(updateCamera);

                if (response == -1)
                {
                    onFailure("Update failed");
                }
                else
                {
                    var cameraId = cameraModel.Id;

                    AllCamerasChanged?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        public Task<AOResult> RemoveCameraAsync(CameraBindableModel cameraModel)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var updateCamera = _mapperService.Map<CameraDTO>(cameraModel);

                await _repositoryService.DeleteAsync(updateCamera);

                AllCamerasChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public Task<AOResult<DahuaResponse<AuthorizationParams>>> AuthorizeAsync(CameraBindableModel camera)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                _dahuaRequestId = 0;
                _dahuaHost = camera.IpAddress;

                var url = $"http://{_dahuaHost}/RPC2_Login";
                var method = "global.login";
                var @params = new Dictionary<string, object>()
                {
                    { Constants.DahuaParametersNames.USERNAME, camera.Login },
                    { Constants.DahuaParametersNames.PASSWORD, string.Empty },
                    { Constants.DahuaParametersNames.CLIENT_TYPE, Constants.DahuaParametersNames.DAHUA_3_WEB_3 },
                };

                var result = await MakeRequestAsync<AuthorizationParams>(method, @params, url: url);

                _dahuaSessionId = result.Session;

                var pwd_phrase = $"{camera.Login}:{result.Params.Realm}:{camera.Password}";
                var pwd_hash = HashConverter.GetMD5FromString(pwd_phrase).ToUpper();
                var pass_phrase = $"{camera.Login}:{result.Params.Random}:{pwd_hash}";
                var pass_hash = HashConverter.GetMD5FromString(pass_phrase).ToUpper();

                @params[Constants.DahuaParametersNames.PASSWORD] = pass_hash;
                @params[Constants.DahuaParametersNames.AUTHORITY_TYPE] = Constants.DahuaParametersNames.DEFAULT;
                @params[Constants.DahuaParametersNames.PASSWORD_TYPE] = Constants.DahuaParametersNames.DEFAULT;

                return await MakeRequestAsync<AuthorizationParams>(method, @params, url: url);
            });
        }

        public Task<AOResult<DahuaResponse<ParamsTable<List<VideoColorConfig>>>>> GetVideoColorConfigsAsync(int channel = 0)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var method = "configManager.getConfig";

                var parameters = new Dictionary<string, object>()
                {
                    { Constants.DahuaParametersNames.NAME, Constants.DahuaParametersNames.VIDEO_COLOR },
                    { Constants.DahuaParametersNames.ONLY_LOCAL, false },
                    { Constants.DahuaParametersNames.CHANNEL, channel },
                };

                return await MakeRequestAsync<ParamsTable<List<VideoColorConfig>>>(method, parameters);
            });
        }

        public Task<AOResult<DahuaResponse<ParamsTable<List<VideoColorConfig>>>>> SetVideoColorConfigsAsync(List<VideoColorConfig> configs, int channel = 0)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var method = "configManager.setConfig";

                var parameters = new Dictionary<string, object>()
                {
                    { Constants.DahuaParametersNames.NAME, Constants.DahuaParametersNames.VIDEO_COLOR },
                    { Constants.DahuaParametersNames.TABLE, configs },
                    { Constants.DahuaParametersNames.OPTIONS, Array.Empty<object>() },
                    { Constants.DahuaParametersNames.CHANNEL, channel },
                };

                return await MakeRequestAsync<ParamsTable<List<VideoColorConfig>>>(method, parameters);
            });
        }

        public Task<AOResult<DahuaResponse<ParamsTable<CameraConfig>>>> GetCameraConfigsAsync(int channel = 0)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var method = "configManager.getConfig";

                var parameters = new Dictionary<string, object>()
                {
                    { Constants.DahuaParametersNames.NAME, Constants.DahuaParametersNames.ENCODE },
                    { Constants.DahuaParametersNames.ONLY_LOCAL, false },
                    { Constants.DahuaParametersNames.CHANNEL, channel },
                };

                return await MakeRequestAsync<ParamsTable<CameraConfig>>(method, parameters);
            });
        }

        public Task<AOResult<DahuaResponse<ParamsTable<CameraConfig>>>> SetCameraConfigsAsync(CameraConfig cameraConfig, int channel = 0)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var method = "configManager.setConfig";

                var parameters = new Dictionary<string, object>()
                {
                    { Constants.DahuaParametersNames.NAME, Constants.DahuaParametersNames.ENCODE },
                    { Constants.DahuaParametersNames.TABLE, cameraConfig },
                    { Constants.DahuaParametersNames.OPTIONS, Array.Empty<object>() },
                    { Constants.DahuaParametersNames.CHANNEL, channel },
                };

                return await MakeRequestAsync<ParamsTable<CameraConfig>>(method, parameters);
            });
        }

        #endregion

        #region -- Private helpers --

        public async Task<DahuaResponse<T>> MakeRequestAsync<T>(string method, object @params = null, string url = null) where T : class
        {
            _dahuaRequestId += 1;

            var data = new Dictionary<string, object>()
            {
                { nameof(method), method },
                { Constants.DahuaParametersNames.ID, _dahuaRequestId },
            };

            if (@params is not null)
            {
                data.Add(nameof(@params), @params);
            }
            if (_dahuaSessionId is not null)
            {
                data.Add(Constants.DahuaParametersNames.SESSION, _dahuaSessionId);
            }

            url ??= $"http://{_dahuaHost}/RPC2";

            var a = await _restService.PostAsync<DahuaResponse<T>>(url, data);

            return a;
        }

        #endregion
    }
}
