namespace SmartMirror.Models
{
    public class NotificationsGroupedByDayModel : List<NotificationModel>
    {
        public NotificationsGroupedByDayModel(
            string name,
            List<NotificationModel> notifications)
            : base(notifications)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
