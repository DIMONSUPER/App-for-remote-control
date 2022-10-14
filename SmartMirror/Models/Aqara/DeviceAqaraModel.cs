namespace SmartMirror.Models.Aqara
{
    public class DeviceAqaraModel
    {
        public string Did { get; set; }

        public string ParentDid { get; set; }

        public string PositionId { get; set; }

        public string DeviceName { get; set; }

        public string Model { get; set; }

        public int ModelType { get; set; }

        public int State { get; set; }

        public long CreateTime { get; set; }

        public string TimeZone { get; set; }

        public long UpdateTime { get; set; }

        public string FirmwareVersion { get; set; }
    }
}
