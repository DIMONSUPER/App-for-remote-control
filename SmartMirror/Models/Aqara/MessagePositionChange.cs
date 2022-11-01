using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class MessagePositionChangeData
{
    [JsonProperty("positionName")]
    public string PositionName { get; set; }

    [JsonProperty("positionId")]
    public string PositionId { get; set; }

    [JsonProperty("dids")]
    public List<string> Dids { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

public class MessagePositionChangeResponse
{
    [JsonProperty("data")]
    public MessagePositionChangeData Data { get; set; }

    [JsonProperty("openId")]
    public string OpenId { get; set; }

    [JsonProperty("msgId")]
    public string MsgId { get; set; }

    [JsonProperty("eventType")]
    public string EventType { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }
}

