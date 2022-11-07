using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Repository;

namespace SmartMirror.Services.Scenarios
{
    public class ScenariosService : IScenariosService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;
        private readonly IAqaraService _aqaraService;
        private readonly IMapperService _mapperService;
        private readonly IDevicesService _devicesService;
        private readonly IAqaraMessanger _aqaraMessanger;
        private readonly IRepositoryService _repositoryService;

        private TaskCompletionSource<object> _scenariosTaskCompletionSource = new();
        private List<ScenarioBindableModel> _allScenarios = new();

        public ScenariosService(
            ISmartHomeMockService smartHomeMockService,
            IAqaraService aqaraService,
            IMapperService mapperService,
            IDevicesService devicesService,
            IRepositoryService repositoryService,
            IAqaraMessanger aqaraMessanger)
        {
            _smartHomeMockService = smartHomeMockService;
            _aqaraService = aqaraService;
            _mapperService = mapperService;
            _devicesService = devicesService;
            _aqaraMessanger = aqaraMessanger;
            _repositoryService = repositoryService;

            _aqaraMessanger.MessageReceived += OnMessageReceived;
        }

        #region -- IScenariosService implementation --

        public event EventHandler AllScenariosChanged;

        public async Task<IEnumerable<ScenarioBindableModel>> GetAllScenariosAsync()
        {
            await _scenariosTaskCompletionSource.Task;

            return _allScenarios;
        }

        public async Task<AOResult> DownloadAllScenariosAsync()
        {
            var result = new AOResult();
            result.SetFailure("Task is already running");

            if (_scenariosTaskCompletionSource.Task.Status is not TaskStatus.RanToCompletion and not TaskStatus.WaitingForActivation and not TaskStatus.Canceled and not TaskStatus.Faulted)
            {
                return result;
            }

            _scenariosTaskCompletionSource = new();

            System.Diagnostics.Debug.WriteLine($"{nameof(DownloadAllScenariosAsync)} STARTED");

            result = await AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var resultOfGettingScenarios = await GetScenariosAsync();

                if (resultOfGettingScenarios.IsSuccess)
                {
                    var bindableModels = _mapperService.MapRange<ScenarioBindableModel>(resultOfGettingScenarios.Result).ToList();

                    await GetSettingsScenariosAsync(bindableModels);

                    var dbModels = _mapperService.MapRange<ScenarioDTO>(bindableModels);

                    await _repositoryService.SaveOrUpdateRangeAsync(dbModels);

                    await GetSettingsScenariosAsync(bindableModels);

                    _allScenarios = new(bindableModels);
                }
                else
                {
                    onFailure("Request failed");
                }
            });

            if (!result.IsSuccess)
            {
                _allScenarios = new();
            }

            AllScenariosChanged?.Invoke(this, EventArgs.Empty);

            System.Diagnostics.Debug.WriteLine($"{nameof(DownloadAllScenariosAsync)} FINISHED");

            _scenariosTaskCompletionSource.TrySetResult(null);

            return result;
        }

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
                    else
                    {
                        onFailure(resultOfGettingScenaries.Message);
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

        public Task<AOResult<ScenarioBindableModel>> GetScenarioByIdAsync(string sceneId)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenario = new ScenarioBindableModel();

                var resultOfGetttingSceneById = await _aqaraService.GetSceneByIdAsync(sceneId);

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

        public Task<AOResult> UpdateScenarioAsync(ScenarioBindableModel bindableScenario)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var updateScenario = _mapperService.Map<ScenarioDTO>(bindableScenario);

                var response = await _repositoryService.SaveOrUpdateAsync(updateScenario);

                if (response == -1)
                {
                    onFailure("Update failed");
                }
                else
                {
                    var bindableScenarioId = bindableScenario.Id;

                    var scenario = _allScenarios.FirstOrDefault(row => row.Id == bindableScenarioId);

                    scenario = bindableScenario;

                    AllScenariosChanged?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        #endregion

        #region -- Private helpers --

        private async Task GetSettingsScenariosAsync(IEnumerable<ScenarioBindableModel> scenarios)
        {
            foreach (var scenario in scenarios)
            {
                var sceneId = scenario.SceneId;

                var dbScenario = await _repositoryService.GetSingleAsync<ScenarioDTO>(row => row.SceneId == sceneId);

                if (dbScenario is not null)
                {
                    scenario.Id = dbScenario.Id;
                    scenario.IsShownInScenarios = dbScenario.IsShownInScenarios;
                    scenario.IsReceiveNotifications = dbScenario.IsReceiveNotifications;
                    scenario.IsFavorite = dbScenario.IsFavorite;
                }
            }
        }

        private void OnMessageReceived(object sender, AqaraMessageEventArgs e)
        {
            if (e.EventType is Constants.Aqara.EventTypes.scene_created or Constants.Aqara.EventTypes.scene_deleted)
            {
                AllScenariosChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private Task<AOResult<IEnumerable<ScenarioModel>>> GetScenariosFromAqaraAsync()
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var scenarios = Enumerable.Empty<ScenarioModel>();

                var resultOfGettingAllScenaries = await _aqaraService.GetScenesAsync();

                if (resultOfGettingAllScenaries.IsSuccess)
                {
                    scenarios = resultOfGettingAllScenaries.Result?.Data?
                        .Select(x => new ScenarioModel
                        {
                            Id = x.SceneId,
                            Name = x.Name,
                        });
                }
                else
                {
                    onFailure(resultOfGettingAllScenaries.Message);
                }

                return scenarios;
            });
        }

        #endregion
    }
}