using System;
using SmartMirror.Enums;

namespace SmartMirror.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EDeviceStatus Status { get; set; }
        public string Type { get; set; }
    }
}

