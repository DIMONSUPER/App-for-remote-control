using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartMirror.Models.Dahua;

public class DahuaResponse<T> : DahuaResponse where T : class
{
    private T _params;

    [JsonIgnore]
    public T Params => _params ??= JsonParams?.ToObject<T>();
}

public class DahuaResponse
{
    [JsonProperty("params")]
    public JToken JsonParams { get; set; }
    public Error Error { get; set; }
    public int? Id { get; set; }
    public bool? Result { get; set; }
    public string Session { get; set; }
}

public class Error
{
    public int? Code { get; set; }
    public string Message { get; set; }
}

