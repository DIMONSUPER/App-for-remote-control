using SmartMirror.Enums;

namespace SmartMirror.Models
{
    public class DeviceModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public EDeviceStatus Status { get; set; }

        public string Type { get; set; }

        public string RoomName { get; set; }
    }
}

