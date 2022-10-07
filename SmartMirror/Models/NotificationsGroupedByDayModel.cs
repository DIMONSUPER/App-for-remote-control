using System.Collections.ObjectModel;

namespace SmartMirror.Models
{
    public class NotificationsGroupedByDayModel : ObservableCollection<NotificationModel>
    {
        public NotificationsGroupedByDayModel(
            string name,
            IEnumerable<NotificationModel> notifications)
            : base(notifications)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
