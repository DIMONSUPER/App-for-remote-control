using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class ChangeValue
{
    [JsonProperty("resourceId")]
    public string ResourceId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class MessageChangeData
{
    [JsonProperty("changeValues")]
    public List<ChangeValue> ChangeValues { get; set; }

    [JsonProperty("time")]
    public long Time { get; set; }

    [JsonProperty("did")]
    public string Did { get; set; }
}

public class MessageChangeResponse
{
    [JsonProperty("data")]
    public MessageChangeData Data { get; set; }

    [JsonProperty("openId")]
    public string OpenId { get; set; }

    [JsonProperty("msgId")]
    public string MsgId { get; set; }

    [JsonProperty("eventType")]
    public string EventType { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

