using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Aqara;

public interface IAqaraService
{
    bool IsAuthorized { get; }

    Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email);

    Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code);

    Task<AOResult<BaseAqaraResponse>> GetAllDevicesAsync();

    Task<AOResult<BaseAqaraResponse<DataAqaraResponse<PositionAqaraModel>>>> GetAllHousesAsync();

    Task<AOResult<BaseAqaraResponse<DataAqaraResponse<PositionAqaraModel>>>> GetAllRoomsAsync(string positionId, int pageNum, int pageSize);
}

