using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class DeviceResponse
{
    [JsonProperty("parentDid")]
    public string ParentDid { get; set; }

    [JsonProperty("createTime")]
    public long CreateTime { get; set; }

    [JsonProperty("timeZone")]
    public string TimeZone { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("updateTime")]
    public long UpdateTime { get; set; }

    [JsonProperty("modelType")]
    public int ModelType { get; set; }

    [JsonProperty("state")]
    public int State { get; set; }

    [JsonProperty("did")]
    public string Did { get; set; }

    [JsonProperty("deviceName")]
    public string DeviceName { get; set; }
}

