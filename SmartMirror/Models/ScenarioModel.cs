namespace SmartMirror.Models
{
    public class ScenarioModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsFavorite { get; set; }

        public DateTime ActivationTime { get; set; }

        public IEnumerable<ScenarioActionModel> ScenarioActions { get; set; }
    }
}