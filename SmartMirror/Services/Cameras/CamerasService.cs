using System.Net.Sockets;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Repository;
using SmartMirror.Services.Rest;

namespace SmartMirror.Services.Cameras
{
    public class CamerasService : ICamerasService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IRepositoryService _repositoryService;
        private readonly IMapperService _mapperService;
        private readonly IRestService _restService;

        public CamerasService(
            ISmartHomeMockService smartHomeMockService,
            IRepositoryService repositoryService,
            IMapperService mapperService,
            IRestService restService)
        {
            _smartHomeMockService = smartHomeMockService;
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

        public Task<AOResult<bool>> CheckCameraConnection(CameraBindableModel camera)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var url = new Uri($"rtsp://{camera.Login}:{camera.Password}@{camera.IpAddress}/live");

                using var rtspClient = new RtspClientSharp.RtspClient(new RtspClientSharp.ConnectionParameters(url));

                try
                {
                    using var tokenSource = new CancellationTokenSource();

                    await rtspClient.ConnectAsync(tokenSource.Token);

                    tokenSource.Cancel();
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

        #endregion
    }
}
