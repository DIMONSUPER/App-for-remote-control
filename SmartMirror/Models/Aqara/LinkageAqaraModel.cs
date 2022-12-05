using SmartMirror.Enums;
using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class LinkageAqaraModel
{
    public string LinkageId { get; set; }

    public string PositionId { get; set; }

    public string Model { get; set; }

    public string Name { get; set; }

    [JsonProperty("localizd")]
    public ELocalizeAqara Localize { get; set; }

    public bool Enable { get; set; }
}