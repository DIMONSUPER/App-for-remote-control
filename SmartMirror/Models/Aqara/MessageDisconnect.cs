using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class MessageDicsonnectData
{
    [JsonProperty("cause")]
    public int Cause { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("did")]
    public string Did { get; set; }
}

public class MessageDicsonnectReponse
{
    [JsonProperty("data")]
    public MessageDicsonnectData Data { get; set; }

    [JsonProperty("openId")]
    public string OpenId { get; set; }

    [JsonProperty("msgId")]
    public string MsgId { get; set; }

    [JsonProperty("eventType")]
    public string EventType { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

