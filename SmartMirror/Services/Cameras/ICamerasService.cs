using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.Dahua;

namespace SmartMirror.Services.Cameras;

public interface ICamerasService
{
    event EventHandler AllCamerasChanged;

    Task<AOResult<IEnumerable<CameraBindableModel>>> GetCamerasAsync();

    Task<AOResult> UpdateCameraAsync(CameraBindableModel cameraModel);

    Task<AOResult<bool>> VerifyCameraIPAddressAsync(string ipAddress, CancellationToken? cancellationToken = null);

    Task<AOResult<bool>> CheckCameraConnection(CameraBindableModel camera, CancellationToken? cancellationToken = null);

    Task<AOResult> RemoveCameraAsync(CameraBindableModel cameraModel);

    Task<AOResult<DahuaResponse<AuthorizationParams>>> AuthorizeAsync(CameraBindableModel camera);

    Task<AOResult<DahuaResponse<ParamsTable<List<VideoColorConfig>>>>> GetVideoColorConfigsAsync(int channel = 0);

    Task<AOResult<DahuaResponse<ParamsTable<List<VideoColorConfig>>>>> SetVideoColorConfigsAsync(List<VideoColorConfig> configs, int channel = 0);

    Task<AOResult<DahuaResponse<ParamsTable<CameraConfig>>>> GetCameraConfigsAsync(int channel = 0);

    Task<AOResult<DahuaResponse<ParamsTable<CameraConfig>>>> SetCameraConfigsAsync(CameraConfig cameraConfig, int channel = 0);
}
