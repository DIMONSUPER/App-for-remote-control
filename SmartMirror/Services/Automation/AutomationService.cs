using Android.App;
using SmartMirror.Helpers;
using SmartMirror.Models.BindableModels;
using SmartMirror.Models.DTO;
using SmartMirror.Models;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Mapper;
using SmartMirror.Services.Repository;

namespace SmartMirror.Services.Automation;

public class AutomationService : IAutomationService
{
    private readonly IAqaraService _aqaraService;
    private readonly IMapperService _mapperService;
    private readonly IAqaraMessanger _aqaraMessanger;
    private readonly IRepositoryService _repositoryService;

    private TaskCompletionSource<object> _automationsTaskCompletionSource = new();
    private List<AutomationDTO> _allAutomations = new();

    public AutomationService(
        IAqaraService aqaraService,
        IMapperService mapperService,
        IAqaraMessanger aqaraMessanger,
        IRepositoryService repositoryService)
    {
        _aqaraService = aqaraService;
        _mapperService = mapperService;
        _aqaraMessanger = aqaraMessanger;
        _repositoryService = repositoryService;

        _aqaraMessanger.MessageReceived += OnMessageReceived;
    }

    #region -- IAutomationService implementation --

    public event EventHandler AllAutomationsChanged;

    public async Task<IEnumerable<AutomationDTO>> GetAllAutomationsAsync()
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
            var resultOfGettingAutomations = await _aqaraService.GetLinkageAsync();

            if (resultOfGettingAutomations.IsSuccess)
            {
                var automations = _mapperService.MapRange<AutomationDTO>(resultOfGettingAutomations.Result.Data).ToList();

                await GetSettingsAutomationsAsync(automations);

                await _repositoryService.SaveOrUpdateRangeAsync(automations);

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

    #endregion

    #region -- Private helpers --

    private async Task GetSettingsAutomationsAsync(IEnumerable<AutomationDTO> automations)
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

