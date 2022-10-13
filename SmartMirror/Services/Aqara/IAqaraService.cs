using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Aqara;

public interface IAqaraService
{
    bool IsAuthorized { get; }
    
    Task<AOResult<BaseAqaraResponse>> SendLoginCodeAsync(string email);
    
    Task<AOResult<BaseAqaraResponse>> LoginWithCodeAsync(string email, string code);
    
    Task<AOResult<BaseAqaraResponse>> GetAllDevicesAsync();
    
    Task<AOResult<DataAqaraResponce<SimpleSceneAqaraModel>>> GetAllScenariesAsync(int pageNumber = 1, int pageSize = 100, string positionId = null);
    
    Task<AOResult<DetailSceneAqaraModel>> GetScenarioByIdAsync(string id);

    Task<AOResult> RunScenarioByIdAsync(string id);
}