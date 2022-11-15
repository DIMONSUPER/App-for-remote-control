using System.Linq;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.Aqara;
using SmartMirror.Models.BindableModels;
using SmartMirror.Services.Aqara;
using SmartMirror.Services.Devices;
using SmartMirror.Services.Mock;
using SmartMirror.Services.Rest;
using SmartMirror.Services.Settings;

namespace SmartMirror.Services.Notifications
{
    public class NotificationsService : BaseAqaraService, INotificationsService
    {
        private readonly IDevicesService _devicesService;
        private readonly IAqaraMessanger _aqaraMessanger;
        private readonly ISettingsManager _settingsManager;

        public NotificationsService(
            IRestService restService,
            ISettingsManager settingsManager,
            IDevicesService devicesService,
            IAqaraMessanger aqaraMessanger)
            : base(restService, settingsManager)
        {
            _devicesService = devicesService;
            _aqaraMessanger = aqaraMessanger;
            _settingsManager = settingsManager;

            _aqaraMessanger.MessageReceived += OnMessageReceived;
        }

        #region -- INotificationsService implementation --

        public event EventHandler<NotificationGroupItemBindableModel> NotificationReceived;

        public bool IsAllowNotifications => _settingsManager.NotificationsSettings.IsAllowNotifications;

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
                    NotificationReceived?.Invoke(this, new NotificationGroupItemBindableModel
                    {
                        Device = device,
                        IsShown = device.IsReceiveNotifications,
                        LastActivityTime = DateTime.Now,
                        Status = aqaraMessage.Value,
                    });
                }
            }
        }

        #endregion
    }
}
