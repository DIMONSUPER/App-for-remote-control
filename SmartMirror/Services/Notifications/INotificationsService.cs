using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Notifications
{
    public interface INotificationsService
    {
        event EventHandler<NotificationGroupItemBindableModel> NotificationReceived;

        bool IsAllowNotifications { get; }

        Task<AOResult<IEnumerable<NotificationGroupItemBindableModel>>> GetNotificationsForDeviceAsync(string deviceId, params string[] resourceIds);
    }
}
