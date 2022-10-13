using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class DetailSceneAqaraModel
    {
        [JsonProperty("localize")]
        public int Localize { get; set; }

        [JsonProperty("sceneId")]
        public string SceneId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("action")]

        public IEnumerable<ActionAqaraModel> Actions;
    }
}
