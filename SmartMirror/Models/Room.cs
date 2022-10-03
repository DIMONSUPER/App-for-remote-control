using System;
namespace SmartMirror.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
        public IEnumerable<Device> Devices { get; set; }
    }
}

