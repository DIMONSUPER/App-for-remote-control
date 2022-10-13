using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Scenarios
{
    public class ScenariosService : IScenariosService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IAqaraService _aqaraService;

        public ScenariosService(
            ISmartHomeMockService smartHomeMockService,
            IAqaraService aqaraService)
        {
            _smartHomeMockService = smartHomeMockService;
            _aqaraService = aqaraService;
        }

        #region -- IScenariosService implementation --

        public Task<AOResult<IEnumerable<ScenarioModel>>> GetAllScenariosAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGettingScenaries = await GetAllScenariosFromAqaraAsync();

                    if (resultOfGettingScenaries.IsSuccess)
                    {
                        scenarios = resultOfGettingScenaries.Result;
                    }

                    var mockScenarios = _smartHomeMockService.GetScenarios();

                    if (mockScenarios is not null)
                    {
                        scenarios = scenarios.Concat(mockScenarios);
                    } 
                }
                else
                {
                    onFailure("Unauthorized");
                }

                return scenarios;
            });
        }

        public Task<AOResult<IEnumerable<ScenarioModel>>> GetFavoriteScenariosAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();
                
                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGettingScenaries = await GetAllScenariosFromAqaraAsync();

                    if (resultOfGettingScenaries.IsSuccess)
                    {
                        scenarios = resultOfGettingScenaries.Result;
                    }
                    else
                    {
                        onFailure(resultOfGettingScenaries.Message);
                    } 
                }
                else
                {
                    onFailure("Unauthorized");
                }

                return scenarios;
            });
        }

        public Task<AOResult<ScenarioModel>> GetScenarioByIdAsync(string sceneId)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenario = new ScenarioModel();

                var resultOfGetttingSceneById = await _aqaraService.GetScenarioByIdAsync(sceneId);

                if (resultOfGetttingSceneById is not null)
                {
                    if (resultOfGetttingSceneById.IsSuccess && resultOfGetttingSceneById.Result is not null)
                    {
                        var scene = resultOfGetttingSceneById?.Result;

                        scenario = new ScenarioModel
                        {
                            Name = scene.Name,
                        };
                    }
                    else
                    {
                        onFailure($"Error: {resultOfGetttingSceneById.Message}");
                    }
                }
                else
                {
                    onFailure("scenarios is null");
                }

                return scenario;
            });
        }

        public Task<AOResult> RunScenarioAsync(string id)
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGettingScenarios = _smartHomeMockService.GetScenarios();

                    if (resultOfGettingScenarios is not null)
                    {
                        var scenario = resultOfGettingScenarios.FirstOrDefault(row => row.Id == id);

                        scenario.IsActive = true;
                    }
                    else
                    {
                        onFailure("scenarios is null");
                    } 
                }
                else
                {
                    onFailure("Unauthorized");
                }

                return Task.CompletedTask;
            });
        }

        #endregion

        #region -- Private helpers --

        private Task<AOResult<IEnumerable<ScenarioModel>>> GetAllScenariosFromAqaraAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGetttingAllScenaries = await _aqaraService.GetAllScenariesAsync();

                if (resultOfGetttingAllScenaries is not null)
                {
                    if (resultOfGetttingAllScenaries.IsSuccess)
                    {
                        scenarios = resultOfGetttingAllScenaries.Result?.Data?
                            .Select(x => new ScenarioModel
                            {
                                Id = x.SceneId,
                                Name = x.Name,
                            });
                    }
                    else
                    {
                        onFailure($"Error: {resultOfGetttingAllScenaries.Message}");
                    }
                }
                else
                {
                    onFailure("scenarios is null");
                }

                return scenarios;
            });
        } 

        #endregion
    }
}
