namespace SmartMirror.Models.Aqara;

public class AccessResponse
{
    public long ExpiresIn { get; set; }

    public string OpenId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}

