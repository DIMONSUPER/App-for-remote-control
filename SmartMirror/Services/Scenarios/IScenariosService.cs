using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Scenarios
{
    public interface IScenariosService
    {
        event EventHandler AllScenariosChanged;

        Task<IEnumerable<ScenarioBindableModel>> GetAllScenariosAsync();

        Task<AOResult> DownloadAllScenariosAsync();

        Task<AOResult<IEnumerable<ScenarioModel>>> GetScenariosAsync();

        Task<AOResult<ScenarioBindableModel>> GetScenarioByIdAsync(string scenarioId);

        Task<AOResult> RunScenarioAsync(string scenarioId);

        Task<AOResult> UpdateScenarioAsync(ScenarioBindableModel scenario);
    }
}