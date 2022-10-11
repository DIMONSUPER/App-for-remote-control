using Newtonsoft.Json;

namespace SmartMirror.Models.Aqara;

public class AccessResponse
{
    [JsonProperty("expiresIn")]
    public long ExpiresIn { get; set; }

    [JsonProperty("openId")]
    public string OpenId { get; set; }

    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; }
}

