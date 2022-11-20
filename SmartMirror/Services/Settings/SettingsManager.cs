namespace SmartMirror.Services.Settings;

public class SettingsManager : ISettingsManager
{
    #region -- ISettingsManager implementation --

    public AqaraAccessSettings AqaraAccessSettings { get; } = new();

    public NotificationsSettings NotificationsSettings { get; } = new();

    #endregion
}

