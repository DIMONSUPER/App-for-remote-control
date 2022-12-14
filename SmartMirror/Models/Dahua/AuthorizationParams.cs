namespace SmartMirror.Models.Dahua;

public class AuthorizationParams
{
    public string Authorization { get; set; }
    public string Encryption { get; set; }
    public string Random { get; set; }
    public string Realm { get; set; }
    public string Time { get; set; }
}

