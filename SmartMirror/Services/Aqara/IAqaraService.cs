using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Aqara;

public interface IAqaraService
{
    bool IsAuthorized { get; }

    Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email);

    Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code);

    Task<AOResult<DataAqaraResponse<DeviceAqaraModel>>> GetDevicesByPositionAsync(string positionId, int pageNum, int pageSize);

    Task<AOResult<DataAqaraResponse<PositionAqaraModel>>> GetPositionsAsync(string positionId, int pageNum, int pageSize);
}

