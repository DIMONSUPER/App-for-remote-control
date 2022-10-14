using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class ActionAqaraModel
    {
        public string SubjectId { get; set; }

        public string ActionDefinitionId { get; set; }

        public string ActionName { get; set; }

        public object Model { get; set; }

        public string DelayTime { get; set; }

        public string DelayTimeUnit { get; set; }

        [JsonProperty("params")]
        public IEnumerable<ActionParameterAqaraModel> ActionParameters { get; set; }
    }
}