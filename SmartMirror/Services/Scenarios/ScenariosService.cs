using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Scenarios
{
    public class ScenariosService : IScenariosService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;

        public ScenariosService(ISmartHomeMockService smartHomeMockService)
        {
            _smartHomeMockService = smartHomeMockService;
        }

        #region -- IScenariosService implementation --

        public Task<AOResult<IEnumerable<ScenarioModel>>> GetAllScenariosAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGettingScenarios = _smartHomeMockService.GetScenarios();

                if (resultOfGettingScenarios is not null)
                {
                    scenarios = resultOfGettingScenarios;
                }
                else
                {
                    onFailure("scenarios is null");
                }

                return Task.FromResult(scenarios);
            });
        }

        public Task<AOResult<IEnumerable<ScenarioModel>>> GetFavoriteScenariosAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGettingScenarios = _smartHomeMockService.GetScenarios();

                if (resultOfGettingScenarios is not null)
                {
                    scenarios = resultOfGettingScenarios.Where(row => row.IsFavorite);
                }
                else
                {
                    onFailure("scenarios is null");
                }

                return Task.FromResult(scenarios);
            });
        }

        public Task<AOResult> UpdateActiveStatusScenarioAsync(int id, bool active)
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var resultOfGettingScenarios = _smartHomeMockService.GetScenarios();

                if (resultOfGettingScenarios is not null)
                {
                    var scenario = resultOfGettingScenarios.FirstOrDefault(row => row.Id == id);

                    scenario.IsActive = active;
                }
                else
                {
                    onFailure("scenarios is null");
                }

                return Task.CompletedTask;
            });
        }

        #endregion
    }
}
