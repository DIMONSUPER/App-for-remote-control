namespace SmartMirror.Services.Settings;

public interface ISettingsManager
{
    AqaraAccessSettings AqaraAccessSettings { get; }

    NotificationsSettings NotificationsSettings { get; }
}

