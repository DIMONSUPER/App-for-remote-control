using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;
using SmartMirror.Interfaces;

namespace SmartMirror.Services.Notifications
{
    public interface INotificationsService
    {
        event EventHandler<NotificationBindableModel> AllNotificationsChanged;

        bool IsAllowNotifications { get; }

        Task<IEnumerable<NotificationBindableModel>> GetAllNotificationsAsync();

        Task<AOResult> ChangeAllowNotificationsAsync(bool state);

        Task<AOResult<IEnumerable<NotificationBindableModel>>> GetNotificationsForDeviceAsync(string deviceId, params string[] resourceIds);

        Task<AOResult> DownloadAllNotificationsAsync();

        IEnumerable<IGroupableCollection> GetNotificationGroups(IEnumerable<NotificationBindableModel> notifications, Func<NotificationBindableModel, string> keySelector);
    }
}