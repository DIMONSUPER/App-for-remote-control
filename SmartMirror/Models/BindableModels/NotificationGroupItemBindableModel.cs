using SmartMirror.Interfaces;

namespace SmartMirror.Models.BindableModels
{
    public class NotificationGroupItemBindableModel : INotificationGroupItemModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime LastActivityTime { get; set; }
    }
}
