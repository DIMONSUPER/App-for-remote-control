using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Models;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Repository;
using SmartMirror.Models.Aqara;
using System.Linq;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Rooms;

namespace SmartMirror.Services.Automation;

public class AutomationService : BaseAqaraService, IAutomationService
{
    private readonly IAqaraService _aqaraService;
    private readonly IMapperService _mapperService;
    private readonly IAqaraMessanger _aqaraMessanger;
    private readonly IRepositoryService _repositoryService;
    private readonly IDevicesService _devicesService;
    private readonly IRoomsService _roomsService;

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
        IRoomsService roomsService)
        : base(restService, settingsManager, navigationService)
    {
        _aqaraService = aqaraService;
        _mapperService = mapperService;
        _aqaraMessanger = aqaraMessanger;
        _repositoryService = repositoryService;
        _devicesService = devicesService;
        _roomsService = roomsService;

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

        _automationsTaskCompletionSource = new();

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
                var bindableConditions = _mapperService.MapRange<ConditionBindableModel>(linkageDetail.Result.Conditions.Condition, (m, vm) =>
                {
                    vm.Device = devices?.FirstOrDefault(x => x.DeviceId == vm.SubjectId);

                    if (vm.Device is not null)
                    {
                        vm.Device.RoomName = rooms.FirstOrDefault(x => x.Id == vm.Device.PositionId)?.Name;
                    }
                });

                var bindableActions = _mapperService.MapRange<ActionBindableModel>(linkageDetail.Result.Actions.Action, (m, vm) =>
                {
                    vm.Device = devices?.FirstOrDefault(x => x.DeviceId == vm.SubjectId);

                    if (vm.Device is not null)
                    {
                        vm.Device.RoomName = rooms.FirstOrDefault(x => x.Id == vm.Device.PositionId)?.Name;
                    }
                });
                
                automation.Conditions = new(bindableConditions);
                automation.Actions = new(bindableActions);
                automation.Description = automation.Conditions.Select(row => row.TriggerName).Aggregate((i, j) => i + ", " + j);
            }
        }
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

