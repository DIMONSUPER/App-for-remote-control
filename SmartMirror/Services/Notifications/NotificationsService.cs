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

        public Task<AOResult<IEnumerable<NotificationsGroupedByDayModel>>> GetNotificationsGroupedByDayAsync()
        {
            var result = new AOResult<IEnumerable<NotificationsGroupedByDayModel>>();

            try
            {
                var notifications = _smartHomeMockService.GetNotifications();

                if (notifications is not null)
                {
                    var notificationsGroupedByDay = notifications
                        .GroupBy(row => row.LastActivityTime.ToString(Constants.Formats.DATE_FORMAT))
                        .Select(group => new NotificationsGroupedByDayModel(group.Key, group.ToList()));

                    result.SetSuccess(notificationsGroupedByDay);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetNotificationsGroupedByDayAsync)}: exception", "SomeIssues", ex);
            }

            return Task.FromResult(result);
        }

        #endregion
    }
}
