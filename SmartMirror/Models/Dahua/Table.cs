using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartMirror.Models.Dahua;

public class ParamsTable<T> : ParamsTable where T : class
{
    private T _table;

    [JsonIgnore]
    public T Table => _table ??= JsonTable?.ToObject<T>();
}

public class ParamsTable
{
    [JsonProperty("table")]
    public JToken JsonTable { get; set; }
}

