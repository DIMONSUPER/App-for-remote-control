using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Automation;
using SmartMirror.Services.Cameras;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Scenarios;
using SmartMirror.Services.Settings;
using SmartMirror.Interfaces;

namespace SmartMirror.Services.Notifications
{
    public class NotificationsService : BaseAqaraService, INotificationsService
    {
        private readonly IDevicesService _devicesService;
        private readonly IAutomationService _automationService;
        private readonly ICamerasService _camerasService;
        private readonly IScenariosService _scenariosService;
        private readonly IAqaraMessanger _aqaraMessanger;
        private readonly ISettingsManager _settingsManager;

        private TaskCompletionSource<object> _notificationsTaskCompletionSource = new();
        private List<NotificationBindableModel> _allNotifications = new();
        private List<NotificationBindableModel> _cachedNotifications = new();

        public NotificationsService(
            IRestService restService,
            ISettingsManager settingsManager,
            IDevicesService devicesService,
            IAutomationService automationService,
            ICamerasService camerasService,
            IScenariosService scenariosService,
            IAqaraMessanger aqaraMessanger,
            INavigationService navigationService)
            : base(restService, settingsManager, navigationService)
        {
            _devicesService = devicesService;
            _automationService = automationService;
            _camerasService = camerasService;
            _scenariosService = scenariosService;
            _aqaraMessanger = aqaraMessanger;
            _settingsManager = settingsManager;

            _aqaraMessanger.MessageReceived += OnMessageReceived;

            _devicesService.AllDevicesChanged += OnAllDevicesChanged;
            _automationService.AllAutomationsChanged += OnAllAutomationsChanged;
            _camerasService.AllCamerasChanged += OnAllCamerasChanged;
            _scenariosService.AllScenariosChanged += OnAllScenariosChanged;
        }

        #region -- INotificationsService implementation --

        public event EventHandler<NotificationBindableModel> AllNotificationsChanged;

        public bool IsAllowNotifications => _settingsManager.NotificationsSettings.IsAllowNotifications;

        public async Task<IEnumerable<NotificationBindableModel>> GetAllNotificationsAsync()
        {
            await _notificationsTaskCompletionSource.Task;

            return _allNotifications;
        }

        public Task<AOResult> ChangeAllowNotificationsAsync(bool state)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                _settingsManager.NotificationsSettings.IsAllowNotifications = state;

                if (state)
                {
                    await FilterNotificationsAsync();
                }
                else
                {
                    _allNotifications = new();
                }

                AllNotificationsChanged?.Invoke(this, null);
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

            result = await AOResult.ExecuteTaskAsync(async onFailure =>
            {
                List<NotificationBindableModel> notifications = new();

                var devices = await _devicesService.GetAllDevicesAsync();

                var supportedDevices = await _devicesService.GetAllSupportedDevicesAsync();

                foreach (var device in devices)
                {
                    var resourceIds = supportedDevices
                        .Where(x => x.DeviceId == device.DeviceId && x.EditableResourceId is not null)
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

                notifications.Sort(Comparer<NotificationBindableModel>.Create((item1, item2) => item2.LastActivityTime.CompareTo(item1.LastActivityTime)));

                _cachedNotifications = new(notifications);

                await FilterNotificationsAsync();
            });

            if (!result.IsSuccess)
            {
                _allNotifications = new();
            }

            AllNotificationsChanged?.Invoke(this, null);

            _notificationsTaskCompletionSource.TrySetResult(null);

            return result;
        }

        public Task<AOResult<IEnumerable<NotificationBindableModel>>> GetNotificationsForDeviceAsync(string deviceId, params string[] resourceIds)
        {
            return AOResult.ExecuteTaskAsync(async onFailure =>
            {
                var result = new List<NotificationBindableModel>();

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
                                var notification = new NotificationBindableModel
                                {
                                    Device = device,
                                    LastActivityTime = DateTimeHelper.ConvertFromMilliseconds(resource.TimeStamp).ToLocalTime(),
                                    Status = resource.Value,
                                };

                                result.Add(notification);
                            }
                        }
                    }
                }

                return result as IEnumerable<NotificationBindableModel>;
            });
        }

        public IEnumerable<IGroupableCollection> GetNotificationGroups(IEnumerable<NotificationBindableModel> notifications, Func<NotificationBindableModel, string> keySelector)
        {
            var groupableCollections = Enumerable.Empty<IGroupableCollection>();

            if (notifications is not null && notifications.Any())
            {
                groupableCollections = notifications
                    .GroupBy(keySelector)
                    .Select(x => new NotificationGroupBindableModel(x.Key, x));
            }

            return groupableCollections;
        }

        #endregion

        #region -- Private helpers --

        private async void OnAllScenariosChanged(object sender, EventArgs e)
        {
            await FilterNotificationsAsync();
        }

        private async void OnAllCamerasChanged(object sender, EventArgs e)
        {
            await FilterNotificationsAsync();
        }

        private async void OnAllAutomationsChanged(object sender, EventArgs e)
        {
            await FilterNotificationsAsync();
        }

        private async void OnAllDevicesChanged(object sender, DeviceBindableModel device)
        {
            await FilterNotificationsAsync();
        }

        private async Task FilterNotificationsAsync()
        {
            _allNotifications = new();

            var devices = await _devicesService.GetAllSupportedDevicesAsync();

            foreach (var notification in _cachedNotifications)
            {
                var deviceId = notification.Device.DeviceId;
                var editableResourceId = notification.Device.EditableResourceId;

                var device = devices.FirstOrDefault(x => x.DeviceId == deviceId && x.EditableResourceId == editableResourceId);

                if (device is not null)
                {
                    notification.IsEmergencyNotification = device.IsEmergencyNotification;
                    notification.IsReceiveNotifications = device.IsReceiveNotifications;
                    notification.IsShown = device.IsReceiveNotifications;
                    notification.Status = device.AdditionalInfo;

                    if (notification.IsReceiveNotifications)
                    {
                        _allNotifications.Add(notification);
                    }
                }
            }
        }

        private async void OnMessageReceived(object sender, AqaraMessageEventArgs aqaraMessage)
        {
            if (aqaraMessage.EventType is Constants.Aqara.EventTypes.resource_report)
            {
                var devices = await _devicesService.GetAllSupportedDevicesAsync();

                var device = devices.FirstOrDefault(x => x.DeviceId == aqaraMessage.DeviceId && x.EditableResourceId == aqaraMessage.ResourceId);

                if (device is not null)
                {
                    var notification = new NotificationBindableModel
                    {
                        Device = device,
                        IsShown = device.IsReceiveNotifications,
                        IsEmergencyNotification = device.IsEmergencyNotification,
                        LastActivityTime = DateTime.Now,
                        Status = aqaraMessage.Value,
                    };

                    _cachedNotifications.Add(notification);

                    if (device.IsReceiveNotifications)
                    {
                        _allNotifications.Add(notification);

                        AllNotificationsChanged?.Invoke(this, notification);
                    }
                }
            }
        }

        #endregion
    }
}
