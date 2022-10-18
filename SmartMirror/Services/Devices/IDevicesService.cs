using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Devices;

public interface IDevicesService
{
    Task<AOResult> DownloadAllDevicesWithSubInfoAsync(string positionId = null, int pageNum = 1, int pageSize = 100);

    Task<AOResult<DataAqaraResponse<DeviceAqaraModel>>> GetDevicesAsync(string positionId = null, int pageNum = 1, int pageSize = 100, params string[] deviceIds);

    Task<AOResult<IEnumerable<DeviceAqaraModel>>> GetSubdevicesForDeviceAsync(string deviceId);

    Task<AOResult<IEnumerable<ResourceResponse>>> GetDeviceAttributeValueAsync(string deviceId, params string[] resourceIds);

    Task<AOResult<BaseAqaraResponse>> UpdateAttributeValueAsync(string deviceId, params (string resourceId, string value)[] resources);

    Task<AOResult<IEnumerable<AttributeAqaraResponse>>> GetAttributesForDeviceModel(string model, string resourceId = null);

    List<DeviceBindableModel> AllDevicesCollection { get; }
}
