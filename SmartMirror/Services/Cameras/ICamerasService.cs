using SmartMirror.Helpers;
using SmartMirror.Models;
using System.Collections;

namespace SmartMirror.Services.Cameras
{
    public interface ICamerasService
    {
        Task<AOResult<IEnumerable<CameraModel>>> GetCamerasAsync();
    }
}
