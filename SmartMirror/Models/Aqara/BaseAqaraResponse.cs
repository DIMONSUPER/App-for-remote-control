using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartMirror.Models.Aqara;

public class BaseAqaraResponse<T> : BaseAqaraResponse
{
    private T _result;
    [JsonIgnore]
    public T Result
    {
        get
        {
            if (JsonResult is not null)
            {
                _result ??= JsonResult.ToObject<T>(); 
            }

            return _result;
        }
    }
}

public class BaseAqaraResponse
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("requestId")]
    public string RequestId { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("msgDetails")]
    public string MsgDetails { get; set; }

    [JsonProperty("result")]
    public JToken JsonResult { get; set; }
}

