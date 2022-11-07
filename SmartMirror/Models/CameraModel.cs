namespace SmartMirror.Models
{
    public class CameraModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsShown { get; set; }

        public bool IsConnected { get; set; }

        public DateTime CreateTime { get; set; }

        public string VideoUrl { get; set; }
    }
}

