namespace SmartMirror.Models
{
    public class RoomModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public string Description { get; set; }

        public int DevicesCount { get; set; }
    }
}

