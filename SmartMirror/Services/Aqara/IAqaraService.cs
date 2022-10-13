using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Aqara;

public interface IAqaraService
{
    Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email);
    Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code);
    Task<AOResult<DevicesResponse>> GetAllDevicesAsync();
    Task<AOResult<BaseAqaraResponse>> UpdateAttributeValueAsync(string deviceId, params (string resourceId, string value)[] resources);
    Task<AOResult<IEnumerable<ResourceResponse>>> GetDeviceAttributeValueAsync(string deviceId, params string[] resourceIds);
    bool IsAuthorized { get; }
}

