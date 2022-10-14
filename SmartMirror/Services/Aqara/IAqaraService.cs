using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Aqara;

public interface IAqaraService
{
    bool IsAuthorized { get; }

    Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email);
    
    Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code);
    
    Task<AOResult<BaseAqaraResponse>> UpdateAttributeValueAsync(string deviceId, params (string resourceId, string value)[] resources);
    
    Task<AOResult<IEnumerable<ResourceResponse>>> GetDeviceAttributeValueAsync(string deviceId, params string[] resourceIds);
    
    Task<AOResult<DataAqaraResponse<PositionAqaraModel>>> GetPositionsAsync(string positionId, int pageNum = 1, int pageSize = 100);

    Task<AOResult<DataAqaraResponse<DeviceAqaraModel>>> GetDevicesByPositionAsync(string positionId = null, int pageNum = 1, int pageSize = 100);
        
    Task<AOResult<DataAqaraResponse<SimpleSceneAqaraModel>>> GetScenesAsync(int pageNumber = 1, int pageSize = 100, string positionId = null);
    
    Task<AOResult<DetailSceneAqaraModel>> GetSceneByIdAsync(string sceneId);
    
    Task<AOResult> RunSceneByIdAsync(string sceneId);
}
