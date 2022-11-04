using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mock;

namespace SmartMirror.Services.Notifications
{
    public class NotificationsService : INotificationsService
    {
        private readonly ISmartHomeMockService _smartHomeMockService;

        public NotificationsService(ISmartHomeMockService smartHomeMockService)
        {
            _smartHomeMockService = smartHomeMockService;
        }

        #region -- INotificationsService implementation --

        public Task<AOResult<IEnumerable<NotificationModel>>> GetNotificationsAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var notifications = Enumerable.Empty<NotificationModel>();

                var resultOfGettingNotifications = _smartHomeMockService.GetNotifications();

                if (resultOfGettingNotifications is not null)
                {
                    notifications = resultOfGettingNotifications.Where(row => row.IsShow);
                }
                else
                {
                    onFailure("notifications is null");
                }

                return Task.FromResult(notifications);
            });
        }

        #endregion
    }
}
