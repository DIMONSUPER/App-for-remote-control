using Newtonsoft.Json;
using static Microsoft.Maui.Controls.Internals.Profile;

namespace SmartMirror.Models.Aqara;

public class MessageEventResponse
{
    [JsonProperty("data")]
    public List<MessageEventData> Data { get; set; }

    [JsonProperty("msgId")]
    public string MsgId { get; set; }

    [JsonProperty("msgType")]
    public string MsgType { get; set; }

    [JsonProperty("openId")]
    public string OpenId { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }
}

public class MessageEventData
{
    [JsonProperty("resourceId")]
    public string ResourceId { get; set; }

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("subjectId")]
    public string SubjectId { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("triggerSource")]
    public TriggerSource TriggerSource { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}

public class TriggerSource
{
    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("type")]
    public int Type { get; set; }
}

