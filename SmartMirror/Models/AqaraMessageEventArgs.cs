namespace SmartMirror.Models;

public class AqaraMessageEventArgs : EventArgs
{
    #region -- Public properties --

    public string EventType { get; set; }

    public string Time { get; set; }

    public string DeviceId { get; set; }

    public string ResourceId { get; set; }

    public string Value { get; set; }

    #endregion
}

