using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using System.Collections;

namespace SmartMirror.Services.Cameras;

public interface ICamerasService
{
    event EventHandler AllCamerasChanged;

    Task<AOResult<IEnumerable<CameraBindableModel>>> GetCamerasAsync();

    Task<AOResult> UpdateCameraAsync(CameraBindableModel cameraModel);

    Task<AOResult<bool>> VerifyCameraIPAddressAsync(string ipAddress);

    Task<AOResult<bool>> CheckCameraConnection(CameraBindableModel camera);

    Task<AOResult> RemoveCameraAsync(CameraBindableModel cameraModel);
}
