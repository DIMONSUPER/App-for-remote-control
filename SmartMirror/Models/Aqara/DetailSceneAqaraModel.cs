using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class DetailSceneAqaraModel
    {
        public string SceneId { get; set; }

        public int Localize { get; set; }

        public string Name { get; set; }

        [JsonProperty("action")]
        public IEnumerable<ActionAqaraModel> Actions { get; set; }
    }
}
