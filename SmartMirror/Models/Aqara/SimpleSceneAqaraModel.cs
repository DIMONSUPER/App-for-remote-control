using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara
{
    public class SimpleSceneAqaraModel
    {
        public int Localizd { get; set; }

        public string SceneId { get; set; }

        public string Name { get; set; }

        public object Model { get; set; }
    }
}
