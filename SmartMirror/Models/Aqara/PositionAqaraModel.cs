namespace SmartMirror.Models.Aqara
{
    public class PositionAqaraModel
    {
        public string PositionId { get; set; }

        public string PositionName { get; set; }

        public long CreateTime { get; set; }

        public string Description { get; set; }

        public string ParentPositionId { get; set; }
    }
}
