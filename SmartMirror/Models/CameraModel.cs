using System;
namespace SmartMirror.Models
{
    public class CameraModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsShow { get; set; }

        public bool IsConnected { get; set; }

        public DateTime CreateTime { get; set; }

        public string VideoUrl { get; set; }
    }
}

