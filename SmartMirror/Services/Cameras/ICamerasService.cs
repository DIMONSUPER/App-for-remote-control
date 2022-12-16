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

    Task<AOResult<DahuaResponse>> AuthorizeAsync(CameraBindableModel camera);

    Task<AOResult<DahuaResponse<ParamsTable<List<VideoColorConfig>>>>> GetVideoColorConfigsAsync(CameraBindableModel camera);

    Task<AOResult<DahuaResponse>> SetVideoColorConfigsAsync(CameraBindableModel camera, List<VideoColorConfig> configs);

    Task<AOResult<DahuaResponse<ParamsTable<CameraConfig>>>> GetCameraConfigsAsync(CameraBindableModel camera);

    Task<AOResult<DahuaResponse>> SetCameraConfigsAsync(CameraBindableModel camera,CameraConfig cameraConfig);

    Task<AOResult<DahuaResponse>> KeepAliveAsync(CameraBindableModel camera);

    void LogoutFromDahua(CameraBindableModel camera);
}
