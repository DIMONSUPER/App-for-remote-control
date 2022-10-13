using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class SimpleSceneAqaraModel
    {
        [JsonProperty("localizd")]
        public int Localizd { get; set; }

        [JsonProperty("sceneId")]
        public string SceneId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("model")]
        public object Model { get; set; }
    }
}
