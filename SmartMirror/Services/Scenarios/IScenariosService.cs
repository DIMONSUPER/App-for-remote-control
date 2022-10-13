using SmartMirror.Helpers;
using SmartMirror.Models;

namespace SmartMirror.Services.Scenarios
{
    public interface IScenariosService
    {
        Task<AOResult<IEnumerable<ScenarioModel>>> GetAllScenariosAsync();

        Task<AOResult<ScenarioModel>> GetScenarioByIdAsync(string id);

        Task<AOResult<IEnumerable<ScenarioModel>>> GetFavoriteScenariosAsync();

        Task<AOResult> UpdateActiveStatusScenarioAsync(int id, bool active);
    }
}