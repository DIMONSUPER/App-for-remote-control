using SmartMirror.Helpers;
using SmartMirror.Models;

namespace SmartMirror.Services.Notifications
{
    public interface INotificationsService
    {
        Task<AOResult<IEnumerable<NotificationModel>>> GetNotificationsAsync();
    }
}
