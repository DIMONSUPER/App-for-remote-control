using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Scenarios
{
    public interface IScenariosService
    {
        Task<AOResult<IEnumerable<ScenarioModel>>> GetScenariosAsync();

        Task<AOResult<IEnumerable<ScenarioModel>>> GetFavoriteScenariosAsync();

        Task<AOResult<ScenarioBindableModel>> GetScenarioByIdAsync(string scenarioId);

        Task<AOResult> RunScenarioAsync(string scenarioId);
    }
}