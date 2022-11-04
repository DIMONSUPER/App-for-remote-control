namespace SmartMirror.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public bool IsShow { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime LastActivityTime { get; set; }
    }
}
