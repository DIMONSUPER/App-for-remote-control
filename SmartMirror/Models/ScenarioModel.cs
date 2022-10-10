namespace SmartMirror.Models
{
    public class ScenarioModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsFavorite { get; set; }

        public DateTime ActivationTime { get; set; }

        public IEnumerable<ScenarioAction> ScenarioActions { get; set; }
    }
}