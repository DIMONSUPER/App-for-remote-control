using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using System.Collections;

namespace SmartMirror.Services.Cameras;

public interface ICamerasService
{
    event EventHandler AllCamerasChanged;

    Task<AOResult<IEnumerable<CameraBindableModel>>> GetCamerasAsync();

    Task<AOResult> UpdateCameraAsync(CameraBindableModel cameraModel);

    Task<AOResult<bool>> VerifyCameraIPAddressAsync(string ipAddress, CancellationToken? cancellationToken = null);

    Task<AOResult<bool>> CheckCameraConnection(CameraBindableModel camera, CancellationToken? cancellationToken = null);

    Task<AOResult> RemoveCameraAsync(CameraBindableModel cameraModel);
}
