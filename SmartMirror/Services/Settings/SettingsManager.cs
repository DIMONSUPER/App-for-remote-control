namespace SmartMirror.Services.Settings;

public class SettingsManager : ISettingsManager
{
    public SettingsManager()
    {
        AqaraAccessSettings = new();
    }

    #region -- ISettingsManager implementation --

    public AqaraAccessSettings AqaraAccessSettings { get; }

    #endregion
}

