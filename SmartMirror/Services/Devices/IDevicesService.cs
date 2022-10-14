using SmartMirror.Helpers;
using SmartMirror.Models;

namespace SmartMirror.Services.Devices
{
    public interface IDevicesService
    {
        Task<AOResult<IEnumerable<DeviceModel>>> GetDevicesAsync(string positionId = null, int pageNum = 1, int pageSize = 100);
    }
}
