using System;
namespace SmartMirror.Models
{
    public class ScenarioAction
    {
        public int Id { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public DateTime ActionTime { get; set; }
    }
}

