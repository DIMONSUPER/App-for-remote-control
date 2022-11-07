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

        #region -- ICamerasService implementation --

        public Task<AOResult<IEnumerable<CameraModel>>> GetCamerasAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var cameras = Enumerable.Empty<CameraModel>();

                var camerasGettingResult = _smartHomeMockService.GetCameras();

                if (camerasGettingResult is not null)
                {
                    cameras = camerasGettingResult.Where(camera => camera.IsShown);
                }
                else
                {
                    onFailure("Cameras is null");
                }

                return Task.FromResult(cameras);
            });
        }

        #endregion
    }
}
