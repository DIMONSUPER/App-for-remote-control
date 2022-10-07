using Com.Amazon.Identity.Auth.Device.Endpoint;
using SmartMirror.Helpers;
using SmartMirror.Models;
using SmartMirror.Services.Mock;
using System.Collections.Generic;

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

        public Task<AOResult<IEnumerable<NotificationsGroupedByDayModel>>> GetNotificationsGroupedByDayAsync()
        {
            return AOResult.ExecuteTaskAsync(onFailure =>
            {
                var notificationsGroupedByDay = Enumerable.Empty<NotificationsGroupedByDayModel>();

                var notifications = _smartHomeMockService.GetNotifications();

                if (notifications is not null)
                {
                    notificationsGroupedByDay = notifications
                        .GroupBy(row => row.LastActivityTime.ToString(Constants.Formats.DATE_FORMAT))
                        .Select(group => new NotificationsGroupedByDayModel(group.Key, group.ToList()));
                }

                return Task.FromResult(notificationsGroupedByDay);
            });
        }

        #endregion
    }
}
