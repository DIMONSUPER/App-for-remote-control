using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class ResourceResponse
    {
        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }
    }
}

