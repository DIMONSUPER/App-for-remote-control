using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Models.BindableModels;

namespace SmartMirror.Services.Notifications
{
    public interface INotificationsService
    {
        event EventHandler<NotificationGroupItemBindableModel> AllNotificationsChanged;

        bool IsAllowNotifications { get; }

        Task<IEnumerable<NotificationGroupItemBindableModel>> GetAllNotificationsAsync();

        Task<AOResult> ChangeAllowNotificationsAsync(bool state);

        Task<AOResult<IEnumerable<NotificationGroupItemBindableModel>>> GetNotificationsForDeviceAsync(string deviceId, params string[] resourceIds);

        Task<AOResult> DownloadAllNotificationsAsync();
    }
}
