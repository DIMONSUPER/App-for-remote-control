using System;
namespace SmartMirror.Models
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime ActivationTime { get; set; }
        public List<ScenarioAction> ScenarioActions { get; set; }
    }
}

