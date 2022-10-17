using SmartMirror.Enums;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
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

        public Task<AOResult<DataAqaraResponse<DeviceModel>>> GetDevicesAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var response = new DataAqaraResponse<DeviceModel>();

                var resultOfGettingDevices = await _aqaraService.GetDevicesAsync(positionId, pageNum, pageSize);

                if (resultOfGettingDevices.IsSuccess)
                {
                    response.Data = resultOfGettingDevices.Result.Data.Select(device => new DeviceModel()
                    {
                        Id = device.Did,
                        Name = device.DeviceName,
                        Status = (EDeviceStatus)device.State,
                        Type = device.ModelType.ToString(),
                    });

                    response.TotalCount = resultOfGettingDevices.Result.TotalCount;
                }
                else
                {
                    onFailure("Request failed");
                }

                return response;
            });
        }
    }
}
