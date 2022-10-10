using System;
namespace SmartMirror.Models
{
    public class ScenarioActionModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public DateTime ActionTime { get; set; }
    }
}