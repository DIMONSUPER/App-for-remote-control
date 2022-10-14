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

                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGetttingSceneById = await _aqaraService.GetSceneByIdAsync(sceneId);

                    if (resultOfGetttingSceneById is not null)
                    {
                        if (resultOfGetttingSceneById.IsSuccess && resultOfGetttingSceneById.Result is not null)
                        {
                            var scene = resultOfGetttingSceneById?.Result;

                            scenario = new ScenarioModel
                            {
                                Id = scene.SceneId,
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
                        onFailure("result is null");
                    } 
                }
                else
                {
                    onFailure("Unauthorized");
                }

                return scenario;
            });
        }

        public Task<AOResult> RunScenarioAsync(string id)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                if (_aqaraService.IsAuthorized)
                {
                    var scenariosFromMock = _smartHomeMockService.GetScenarios();

                    if (scenariosFromMock is not null)
                    {
                        var scenario = scenariosFromMock.FirstOrDefault(row => row.Id == id);

                        if (scenario is not null)
                        {
                            scenario.IsActive = true;

                            await Task.Delay(250);
                        }
                        else
                        {
                            var resultOfRunningScene = await _aqaraService.RunSceneByIdAsync(id);

                            if (!resultOfRunningScene.IsSuccess)
                            {
                                onFailure(resultOfRunningScene.Message);
                            }
                        }
                    }
                }
                else
                {
                    onFailure("Unauthorized");
                }
            });
        }

        #endregion

        #region -- Private helpers --

        private Task<AOResult<IEnumerable<ScenarioModel>>> GetAllScenariosFromAqaraAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGettingAllScenaries = await _aqaraService.GetAllScenesAsync();

                if (resultOfGettingAllScenaries is not null)
                {
                    if (resultOfGettingAllScenaries.IsSuccess)
                    {
                        scenarios = resultOfGettingAllScenaries.Result?.Data?
                            .Select(x => new ScenarioModel
                            {
                                Id = x.SceneId,
                                Name = x.Name,
                                IsFavorite = true,
                            });
                    }
                    else
                    {
                        onFailure($"Error: {resultOfGettingAllScenaries.Message}");
                    }
                }
                else
                {
                    onFailure("result is null");
                }

                return scenarios;
            });
        } 

        #endregion
    }
}