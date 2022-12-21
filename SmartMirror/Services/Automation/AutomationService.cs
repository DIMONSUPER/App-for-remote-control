using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Models;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Repository;
using SmartMirror.Models.Aqara;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Rooms;
using SmartMirror.Resources;
using Android.App;
using SmartMirror.Services.Scenarios;

namespace SmartMirror.Services.Automation;

public class AutomationService : BaseAqaraService, IAutomationService
{
    private readonly IAqaraService _aqaraService;
    private readonly IMapperService _mapperService;
    private readonly IAqaraMessanger _aqaraMessanger;
    private readonly IRepositoryService _repositoryService;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;
    private readonly IScenariosService _scenariosService;

    private List<AutomationBindableModel> _allAutomations = new();

    private TaskCompletionSource<object> _automationsTaskCompletionSource = new();

    public AutomationService(
        IRestService restService,
        ISettingsManager settingsManager,
        INavigationService navigationService,
        IAqaraService aqaraService,
        IMapperService mapperService,
        IAqaraMessanger aqaraMessanger,
        IRepositoryService repositoryService,
        IDevicesService devicesService,
        IScenariosService scenariosService,
        IRoomsService roomsService)
        : base(restService, settingsManager, navigationService)
    {
        _aqaraService = aqaraService;
        _mapperService = mapperService;
        _aqaraMessanger = aqaraMessanger;
        _repositoryService = repositoryService;
        _devicesService = devicesService;
        _roomsService = roomsService;
        _scenariosService = scenariosService;

        _aqaraMessanger.MessageReceived += OnMessageReceived;
    }

    #region -- IAutomationService implementation --

    public event EventHandler AllAutomationsChanged;

    public Task<AOResult<DataAqaraResponse<LinkageAqaraModel>>> GetLinkagesAsync(string positionId = null, int pageNum = 1, int pageSize = 100)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var requestData = new
            {
                PositionId = positionId,
                pageNum = pageNum,
                pageSize = pageSize,
            };

            var response = await MakeRequestAsync<DataAqaraResponse<LinkageAqaraModel>>("query.linkage.listByPositionId", requestData, onFailure);

            return response.Result;
        });
    }

    public Task<AOResult<BaseAqaraResponse>> ChangeLinkageStateAsync(string linkageId, bool isEnabled)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var data = new
            {
                linkageId = linkageId,
                enable = isEnabled ? 1 : 0,
            };

            var response = await MakeRequestAsync("config.linkage.enable", data, onFailure);

            return response;
        });
    }

    public Task<AOResult<LinkageDetailAqaraModel>> GetLinkageDetailAsync(string linkageId)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var requestData = new
            {
                linkageId = linkageId,
            };

            var response = await MakeRequestAsync<LinkageDetailAqaraModel>("query.linkage.detail", requestData, onFailure);

            return response.Result;
        });
    }

    public async Task<IEnumerable<AutomationBindableModel>> GetAllAutomationsAsync()
    {
        await _automationsTaskCompletionSource.Task;

        return _allAutomations;
    }

    public async Task<AOResult> DownloadAllAutomationsAsync()
    {
        var result = new AOResult();

        result.SetFailure("Task is already running");

        if (_automationsTaskCompletionSource.Task.Status is not TaskStatus.RanToCompletion and not TaskStatus.WaitingForActivation and not TaskStatus.Canceled and not TaskStatus.Faulted)
        {
            return result;
        }

        result = await AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var resultOfGettingAutomations = await GetLinkagesAsync();

            if (resultOfGettingAutomations.IsSuccess)
            {
                var automations = _mapperService.MapRange<AutomationBindableModel>(resultOfGettingAutomations.Result.Data).ToList();

                await GetDetailAutomationsAsync(automations);
                await GetSettingsAutomationsAsync(automations);

                var dbModels = _mapperService.MapRange<AutomationDTO>(automations);

                await _repositoryService.SaveOrUpdateRangeAsync(dbModels);

                //This is required! After first adding automation id = 0, it is required to update to the real
                await GetSettingsAutomationsAsync(automations);

                _allAutomations = new(automations);
            }
            else
            {
                onFailure("Request failed");
            }
        });

        if (!result.IsSuccess)
        {
            _allAutomations = new();
        }

        AllAutomationsChanged?.Invoke(this, EventArgs.Empty);

        _automationsTaskCompletionSource.TrySetResult(null);

        return result;
    }

    public Task<AOResult> UpdateAutomationAsync(AutomationBindableModel bindableAutomation)
    {
        return AOResult.ExecuteTaskAsync(async onFailure =>
        {
            var updateAutomation = _mapperService.Map<AutomationDTO>(bindableAutomation);

            var response = await _repositoryService.SaveOrUpdateAsync(updateAutomation);

            if (response == -1)
            {
                onFailure("Update failed");
            }
            else
            {
                var bindableAutomationId = bindableAutomation.Id;

                var automation = _allAutomations.FirstOrDefault(row => row.Id == bindableAutomationId);

                automation = bindableAutomation;

                AllAutomationsChanged?.Invoke(this, EventArgs.Empty);
            }
        });
    }

    #endregion

    #region -- Private helpers --

    private async Task GetDetailAutomationsAsync(IEnumerable<AutomationBindableModel> automations)
    {
        foreach (var automation in automations)
        {
            var linkageDetail = await GetLinkageDetailAsync(automation.LinkageId);
            var devices = await _devicesService.GetAllDevicesAsync();
            var rooms = await _roomsService.GetAllRoomsAsync();

            if (linkageDetail.IsSuccess)
            {
                var bindableConditions = _mapperService.MapRange<ConditionAqaraModel ,ConditionBindableModel>(linkageDetail.Result.Conditions.Condition, async (m, vm) =>
                {
                    vm.Device = devices?.FirstOrDefault(x => x.DeviceId == vm.SubjectId);

                    if (vm.Device is not null)
                    {
                        vm.Device.RoomName = rooms.FirstOrDefault(x => x.Id == vm.Device.PositionId)?.Name;
                    }

                    await SetAdditionalInfoForConditionAsync(vm);
                });

                var bindableActions = _mapperService.MapRange<ActionAqaraModel, ActionBindableModel>(linkageDetail.Result.Actions.Action, async (m, vm) =>
                {
                    vm.Device = devices?.FirstOrDefault(x => x.DeviceId == vm.SubjectId);

                    if (vm.Device is not null)
                    {
                        vm.Device.RoomName = rooms.FirstOrDefault(x => x.Id == vm.Device.PositionId)?.Name;
                    }

                    await SetAdditionalInfoForActionAsync(vm);
                });

                automation.Relation = linkageDetail.Result.Conditions.Relation;
                automation.Conditions = new(bindableConditions);
                automation.Actions = new(bindableActions);
                automation.Description = automation.Conditions.Select(row => row.TriggerName).Aggregate((i, j) => i + ", " + j);
            }
        }
    }

    private async Task SetAdditionalInfoForConditionAsync(ConditionBindableModel condition)
    {
        if (condition.Device is null)
        {
            condition.IconSource = IconsNames.pic_gears;
            condition.DeviceName = ModelsNames.GetName(condition.Model);
        }
        else
        {
            condition.IconSource = condition.Device.IconSource;
            condition.DeviceName = condition.Device.Name;
            condition.RoomName = condition.Device.RoomName;
        }

        condition.Condition = await GetDescriptionForConditionAsync(condition);

        condition.Condition = SetDescriptionCondition(condition.Condition, condition.Params);
    }

    private async Task SetAdditionalInfoForActionAsync(ActionBindableModel action)
    {
        if (action.Device is null)
        {
            action.IconSource = IconsNames.pic_gears;
            action.DeviceName = ModelsNames.GetName(action.Model);
        }
        else
        {
            action.IconSource = action.Device.IconSource;
            action.DeviceName = action.Device.Name;
            action.RoomName = action.Device.RoomName;
        }

        action.Condition = await GetDescriptionForActionAsync(action);

        action.Condition = SetDescriptionCondition(action.Condition, action.Params);
    }

    private string SetDescriptionCondition(string description, List<ParamAqaraModel> parameters)
    {
        if (description != string.Empty)
        {
            description += " ";
        }

        description += GetDescriptionForParams(parameters ?? new());

        return description;
    }

    private async Task<string> GetNameScenarioAsync(string id)
    {
        var scenarios = await _scenariosService.GetAllScenariosAsync();

        var scenario = scenarios.FirstOrDefault(row => row.SceneId == id);

        if (scenario is null)
        {
            var resultOfGettingScenario = await _scenariosService.GetScenarioByIdAsync(id);

            if (resultOfGettingScenario.IsSuccess)
            {
                scenario = resultOfGettingScenario.Result;
            }
        }

        return scenario?.Name ?? string.Empty;
    }

    private async Task<string> GetNameAutomationAsync(string id)
    {
        var result = string.Empty;

        var automation = _allAutomations.FirstOrDefault(row => row.LinkageId == id);

        if (automation is null)
        {
            var resultOfGettingAutomation = await GetLinkageDetailAsync(id);

            if (resultOfGettingAutomation.IsSuccess)
            {
                result = resultOfGettingAutomation.Result.Name;
            }
        }
        else
        {
            result = automation.Name;
        }

        return result;
    }

    private async Task<string> GetDescriptionForConditionAsync(ConditionBindableModel condition)
    {
        var result = condition.Model switch
        {
            Constants.Aqara.Models.APP_IFTTT_V1 => await GetNameAutomationAsync(condition.SubjectId),
            _ => string.Empty,
        };

        return result;
    }

    private async Task<string> GetDescriptionForActionAsync(ActionBindableModel action)
    {
        var result = action.Model switch
        {
            Constants.Aqara.Models.APP_IFTTT_V1 => await GetNameAutomationAsync(action.SubjectId),
            Constants.Aqara.Models.APP_SCENE_V1 => await GetNameScenarioAsync(action.SubjectId),
            _ => string.Empty,
        };

        return result;
    }

    private string GetDescriptionForParams(List<ParamAqaraModel> parameters)
    {
        var result = string.Empty;

        if (parameters is not null)
        {
            foreach (var param in parameters)
            {
                result += param.ParamId switch
                {
                    Constants.Aqara.ParamDefinition.PD_PHONE_MODEL => $"{param.Value}, ",
                    Constants.Aqara.ParamDefinition.PD_MAP_INFO => $"{param.Value}, ",
                    Constants.Aqara.ParamDefinition.PD_NICK_NAME => $"{param.Value}, ",
                    Constants.Aqara.ParamDefinition.PD_TEMP => $"{int.Parse(param.Value) / (param.ParamType == "0" ? 100 : 1)} {param.ParamUnit}, ",
                    Constants.Aqara.ParamDefinition.PD_HUMI => $"{param.Value}{param.ParamUnit}, ",
                    Constants.Aqara.ParamDefinition.PD_TIMER => $"{DateTimeHelper.ConvertFromAqara(param.Value)}, ",
                    _ => string.Empty,
                };
            }

            if (result != string.Empty)
            {
                result = result.Substring(0, result.Length - 2);
            }
        }

        return result;
    }

    private async Task GetSettingsAutomationsAsync(IEnumerable<AutomationBindableModel> automations)
    {
        foreach (var automation in automations)
        {
            var automationId = automation.LinkageId;

            var dbAutomation = await _repositoryService.GetSingleAsync<AutomationDTO>(row => row.LinkageId == automationId);

            if (dbAutomation is not null)
            {
                automation.Id = dbAutomation.Id;
                automation.IsShownInAutomations = dbAutomation.IsShownInAutomations;
                automation.IsReceiveNotifications = dbAutomation.IsReceiveNotifications;
                automation.IsFavorite = dbAutomation.IsFavorite;
                automation.IsEmergencyNotification = dbAutomation.IsEmergencyNotification;
            }
        }
    }

    private void OnMessageReceived(object sender, AqaraMessageEventArgs e)
    {
        if (e.EventType is Constants.Aqara.EventTypes.linkage_created or Constants.Aqara.EventTypes.linkage_deleted)
        {
            AllAutomationsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion
}

