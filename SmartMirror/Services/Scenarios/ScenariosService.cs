using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Scenarios
{
    public class ScenariosService : IScenariosService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IAqaraService _aqaraService;
        private readonly IMapperService _mapperService;
        private readonly IDevicesService _devicesService;
        private readonly IAqaraMessanger _aqaraMessanger;

        public ScenariosService(
            ISmartHomeMockService smartHomeMockService,
            IAqaraService aqaraService,
            IMapperService mapperService,
            IDevicesService devicesService,
            IAqaraMessanger aqaraMessanger)
        {
            _smartHomeMockService = smartHomeMockService;
            _aqaraService = aqaraService;
            _mapperService = mapperService;
            _devicesService = devicesService;
            _aqaraMessanger = aqaraMessanger;

            _aqaraMessanger.MessageReceived += OnMessageReceived;
        }

        #region -- IScenariosService implementation --

        public event EventHandler ScenariosChanged;

        public Task<AOResult<IEnumerable<ScenarioModel>>> GetScenariosAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                if (_aqaraService.IsAuthorized)
                {
                    var resultOfGettingScenaries = await GetScenariosFromAqaraAsync();

                    if (resultOfGettingScenaries.IsSuccess)
                    {
                        scenarios = resultOfGettingScenaries.Result;
                    }
                }
                else
                {
                    var mockScenarios = _smartHomeMockService.GetScenarios();

                    if (mockScenarios is not null)
                    {
                        scenarios = mockScenarios;
                    }
                }

                return scenarios;
            });
        }

        public Task<AOResult<IEnumerable<ScenarioModel>>> GetFavoriteScenariosAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGettingScenaries = await GetScenariosFromAqaraAsync();

                if (resultOfGettingScenaries.IsSuccess)
                {
                    scenarios = resultOfGettingScenaries.Result;
                }
                else
                {
                    onFailure(resultOfGettingScenaries.Message);
                }

                return scenarios;
            });
        }

        public Task<AOResult<ScenarioBindableModel>> GetScenarioByIdAsync(string sceneId)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenario = new ScenarioBindableModel();

                var resultOfGetttingSceneById = await _aqaraService.GetSceneByIdAsync(sceneId);

                if (resultOfGetttingSceneById is not null)
                {
                    if (resultOfGetttingSceneById.IsSuccess && resultOfGetttingSceneById.Result is not null)
                    {
                        var scene = resultOfGetttingSceneById?.Result;

                        scenario = _mapperService.Map<ScenarioBindableModel>(scene);

                        foreach (var action in scenario.Actions)
                        {
                            action.Device = _devicesService.AllDevices.FirstOrDefault(x => x.DeviceId == action.SubjectId);
                        }
                    }
                    else
                    {
                        onFailure(resultOfGetttingSceneById.Message);
                    }
                }
                else
                {
                    onFailure("result is null");
                }

                return scenario;
            });
        }

        public Task<AOResult> RunScenarioAsync(string id)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
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
            });
        }

        #endregion

        #region -- Private helpers --

        private void OnMessageReceived(object sender, AqaraMessageEventArgs e)
        {
            if (e.EventType is Constants.Aqara.EventTypes.scene_created or Constants.Aqara.EventTypes.scene_deleted)
            {
                ScenariosChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private Task<AOResult<IEnumerable<ScenarioModel>>> GetScenariosFromAqaraAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGettingAllScenaries = await _aqaraService.GetScenesAsync();

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
                        onFailure(resultOfGettingAllScenaries.Message);
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