using System;
namespace SmartMirror.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastActivityTime { get; set; }
    }
}

