using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Devices
{
    public interface IDevicesService
    {
        Task<AOResult<DataAqaraResponse<DeviceModel>>> GetDevicesAsync(string positionId = null, int pageNum = 1, int pageSize = 100);
    }
}
