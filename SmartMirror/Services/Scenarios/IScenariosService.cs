using SmartMirror.Helpers;
using SmartMirror.Models;

namespace SmartMirror.Services.Scenarios
{
    public interface IScenariosService
    {
        Task<AOResult<IEnumerable<ScenarioModel>>> GetAllScenariosAsync();

        Task<AOResult<IEnumerable<ScenarioModel>>> GetFavoriteScenariosAsync();

        Task<AOResult<ScenarioModel>> GetScenarioByIdAsync(string scenarioId);

        Task<AOResult> RunScenarioAsync(string scenarioId);
    }
}