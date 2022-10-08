using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Cameras
{
    public class CamerasService : ICamerasService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;

        public CamerasService(ISmartHomeMockService smartHomeMockService)
        {
            _smartHomeMockService = smartHomeMockService;
        }

        #region -- ICamerasService implementation

        public Task<AOResult<IEnumerable<CameraModel>>> GetCamerasAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var cameras = Enumerable.Empty<CameraModel>();

                var camerasGettingResult = _smartHomeMockService.GetCameras();

                if (camerasGettingResult is not null)
                {
                    cameras = camerasGettingResult;
                }
                else
                {
                    onFailure("Camerass is null");
                }

                return Task.FromResult(cameras);
            });
        }

        #endregion
    }
}
