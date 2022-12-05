using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Notifications
{
    public class NotificationsService : BaseAqaraService, INotificationsService
    {
        private readonly IDevicesService _devicesService;
        private readonly IAqaraMessanger _aqaraMessanger;
        private readonly ISettingsManager _settingsManager;

        private TaskCompletionSource<object> _notificationsTaskCompletionSource = new();
        private List<NotificationGroupItemBindableModel> _allNotifications = new();

        public NotificationsService(
            IRestService restService,
            ISettingsManager settingsManager,
            IDevicesService devicesService,
            IAqaraMessanger aqaraMessanger,
            INavigationService navigationService)
            : base(restService, settingsManager, navigationService)
        {
            _devicesService = devicesService;
            _aqaraMessanger = aqaraMessanger;
            _settingsManager = settingsManager;

            _aqaraMessanger.MessageReceived += OnMessageReceived;
        }

        #region -- INotificationsService implementation --

        public event EventHandler<NotificationGroupItemBindableModel> AllNotificationsChanged;

        public bool IsAllowNotifications => _settingsManager.NotificationsSettings.IsAllowNotifications;

        public async Task<IEnumerable<NotificationGroupItemBindableModel>> GetAllNotificationsAsync()
        {
            await _notificationsTaskCompletionSource.Task;

            return _allNotifications;
        }

        public Task<AOResult> ChangeAllowNotificationsAsync(bool state)
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                _settingsManager.NotificationsSettings.IsAllowNotifications = state;

                return Task.CompletedTask;
            });
        }

        public async Task<AOResult> DownloadAllNotificationsAsync()
        {
            var result = new AOResult();
            result.SetFailure("Task is already running");

            if (_notificationsTaskCompletionSource.Task.Status is not TaskStatus.RanToCompletion and not TaskStatus.WaitingForActivation and not TaskStatus.Canceled and not TaskStatus.Faulted)
            {
                return result;
            }

            _notificationsTaskCompletionSource = new();

            result = await AOResult.ExecuteTaskAsync(async onFailure =>
            {
                List<NotificationGroupItemBindableModel> notifications = new();

                var devices = await _devicesService.GetAllDevicesAsync();

                var supportedDevices = await _devicesService.GetAllSupportedDevicesAsync();

                foreach (var device in devices)
                {
                    var resourceIds = supportedDevices
                        .Where(x => x.IsReceiveNotifications && x.DeviceId == device.DeviceId && x.EditableResourceId is not null)
                        .Select(x => x.EditableResourceId)
                        .ToArray();

                    if (!resourceIds.Any()) continue;

                    var resultOfGettingNotifications = await GetNotificationsForDeviceAsync(device.DeviceId, resourceIds);

                    if (resultOfGettingNotifications.IsSuccess)
                    {
                        notifications.AddRange(resultOfGettingNotifications.Result);
                    }
                    else
                    {
                        onFailure("Request failed");
                    }
                }

                notifications.Sort(Comparer<NotificationGroupItemBindableModel>.Create((item1, item2) => item2.LastActivityTime.CompareTo(item1.LastActivityTime)));

                _allNotifications = new(notifications);
            });

            if (!result.IsSuccess)
            {
                _allNotifications = new();
            }

            AllNotificationsChanged?.Invoke(this, null);

            _notificationsTaskCompletionSource.TrySetResult(null);

            return result;
        }

        public Task<AOResult<IEnumerable<NotificationGroupItemBindableModel>>> GetNotificationsForDeviceAsync(string deviceId, params string[] resourceIds)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var result = new List<NotificationGroupItemBindableModel>();

                var data = new
                {
                    subjectId = deviceId,
                    resourceIds = resourceIds,
                    startTime = DateTimeHelper.ConvertToMilliseconds(DateTime.UtcNow.AddDays(-7)),
                    size = 300,
                };

                var response = await MakeRequestAsync<DataAqaraResponse<ResourceResponse>>("fetch.resource.history", data, onFailure);

                if (response?.Result?.Data is null)
                {
                    onFailure("notifications is null");
                }
                else
                {
                    var devices = await _devicesService.GetAllSupportedDevicesAsync();

                    foreach (var device in devices)
                    {
                        var resources = response.Result.Data.Where(x => x.SubjectId == device.DeviceId && x.ResourceId == device.EditableResourceId);

                        if (resources.Any())
                        {
                            foreach (var resource in resources)
                            {
                                var notification = new NotificationGroupItemBindableModel
                                {
                                    Device = device,
                                    IsShown = device.IsReceiveNotifications,
                                    LastActivityTime = DateTimeHelper.ConvertFromMilliseconds(resource.TimeStamp).ToLocalTime(),
                                    Status = resource.Value,
                                };

                                result.Add(notification);
                            }
                        }
                    }
                }

                return result as IEnumerable<NotificationGroupItemBindableModel>;
            });
        }

        #endregion

        #region -- Private helpers --

        private async void OnMessageReceived(object sender, AqaraMessageEventArgs aqaraMessage)
        {
            if (aqaraMessage.EventType is Constants.Aqara.EventTypes.resource_report)
            {
                var devices = await _devicesService.GetAllSupportedDevicesAsync();

                var device = devices.FirstOrDefault(x => x.IsReceiveNotifications && x.DeviceId == aqaraMessage.DeviceId && x.EditableResourceId == aqaraMessage.ResourceId);

                if (device is not null)
                {
                    var notification = new NotificationGroupItemBindableModel
                    {
                        Device = device,
                        IsShown = device.IsReceiveNotifications,
                        IsEmergencyNotification = device.IsEmergencyNotification,
                        LastActivityTime = DateTime.Now,
                        Status = aqaraMessage.Value,
                    };

                    _allNotifications.Add(notification);

                    AllNotificationsChanged?.Invoke(this, notification);
                }
            }
        }

        #endregion
    }
}
