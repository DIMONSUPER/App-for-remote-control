using SmartMirror.Helpers;
using SmartMirror.Models;
using System.Collections;

namespace SmartMirror.Services.Cameras;

public interface ICamerasService
{
    public event EventHandler AllCamerasChanged;

    Task<AOResult<IEnumerable<CameraModel>>> GetCamerasAsync();

    Task<AOResult> UpdateCameraAsync(CameraModel cameraModel);
}
