using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Aqara;

namespace SmartMirror.Services.Devices
{
    public class DevicesService : IDevicesService
    {
        private readonly IAqaraService _aqaraService;

        public DevicesService(IAqaraService aqaraService)
        {
            _aqaraService = aqaraService;
        }

        public Task<AOResult<IEnumerable<DeviceModel>>> GetDevicesByPositionAsync(string positionId, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var devices = Enumerable.Empty<DeviceModel>();

                var resultOfGettingDevices = await _aqaraService.GetDevicesByPositionAsync(positionId);

                if (resultOfGettingDevices.IsSuccess)
                {
                    devices = resultOfGettingDevices.Result.Data.Select(device => new DeviceModel()
                    {
                        Id = device.Did,
                        Name = device.DeviceName,
                        Status = (Enums.EDeviceStatus)device.State,
                        Type = device.ModelType.ToString(),
                    });
                }
                else
                {
                    onFailure("Request failed");
                }

                return devices;
            });
        }
    }
}
