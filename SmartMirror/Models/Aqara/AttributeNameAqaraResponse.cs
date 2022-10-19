using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class AttributeNameAqaraResponse
{
    [JsonProperty("resourceId")]
    public string ResourceId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("subjectId")]
    public string SubjectId { get; set; }
}

