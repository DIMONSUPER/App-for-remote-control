using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class ActionParameterAqaraModel
    {
        [JsonProperty("paramType")]
        public string ParamType { get; set; }

        [JsonProperty("paramUnit")]
        public string ParamUnit { get; set; }

        [JsonProperty("paramId")]
        public string ParamId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
