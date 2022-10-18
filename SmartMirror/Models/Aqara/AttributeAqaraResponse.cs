using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class AttributeAqaraResponse
{
    [JsonProperty("enums")]
    public string Enums { get; set; }

    [JsonProperty("resourceId")]
    public string ResourceId { get; set; }

    [JsonProperty("minValue")]
    public int? MinValue { get; set; }

    [JsonProperty("unit")]
    public int? Unit { get; set; }

    [JsonProperty("access")]
    public int? Access { get; set; }

    [JsonProperty("maxValue")]
    public int? MaxValue { get; set; }

    [JsonProperty("defaultValue")]
    public string DefaultValue { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }
}

