using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class ActionAqaraModel
    {
        [JsonProperty("delayTimeUnit")]
        public string DelayTimeUnit { get; set; }

        [JsonProperty("actionDefinitionId")]
        public string ActionDefinitionId { get; set; }

        [JsonProperty("model")]
        public object Model { get; set; }

        [JsonProperty("delayTime")]
        public string DelayTime { get; set; }

        [JsonProperty("params")]
        public IEnumerable<ActionParameterAqaraModel> ActionParameters { get; set; }

        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }

        [JsonProperty("actionName")]
        public string ActionName { get; set; }
    }
}
