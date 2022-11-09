using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Repository;

namespace SmartMirror.Services.Cameras
{
    public class CamerasService : ICamerasService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IRepositoryService _repositoryService;
        private readonly IMapperService _mapperService;

        public CamerasService(
            ISmartHomeMockService smartHomeMockService,
            IRepositoryService repositoryService,
            IMapperService mapperService)
        {
            _smartHomeMockService = smartHomeMockService;
            _repositoryService = repositoryService;
            _mapperService = mapperService;
        }

        #region -- ICamerasService implementation --

        public event EventHandler AllCamerasChanged;

        public Task<AOResult<IEnumerable<CameraModel>>> GetCamerasAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var cameras = Enumerable.Empty<CameraModel>();

                var camerasDb = await _repositoryService.GetAllAsync<CameraDTO>();

                if (!camerasDb.Any())
                {
                    var camerasGettingResult = _smartHomeMockService.GetCameras();

                    if (camerasGettingResult is not null)
                    {
                        await _repositoryService.SaveOrUpdateRangeAsync(_mapperService.MapRange<CameraDTO>(camerasGettingResult));
                        cameras = camerasGettingResult;
                    }
                    else
                    {
                        onFailure("Cameras is null");
                    }
                }
                else
                {
                    cameras = _mapperService.MapRange<CameraModel>(camerasDb);
                }

                return cameras;
            });
        }

        public Task<AOResult> UpdateCameraAsync(CameraModel cameraModel)
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

        #endregion
    }
}
