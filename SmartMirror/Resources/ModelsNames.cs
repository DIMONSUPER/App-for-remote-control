namespace SmartMirror.Resources;

public static class ModelsNames
{
    private static Dictionary<string, string> _models { get; set; } = new()
    {
        { "app.timer.v1", "Timer" },
        { "app.ifttt.v1", "If" },
        { "app.ifttt.position_user_alert", "If alert" },
        { "app.geofence.trigger", "Geofence" },
        { "app.weather.humidity", "Weather humidity" },
        { "app.weather.sun", "Weather sun" },
        { "app.weather.temperature", "Weather temperature" },
        { "app.weather.phenomenon", "Weather phenomenon" },
        { "app.geofence.forecast", "Geofence forecast" },
        { "app.scene.v1", "Scene" },
        { "app.mobilepush.v1", "Push notification" },
    };

    public static string GetName(string model)
    {
        return _models.ContainsKey(model)
            ? _models[model]
            : model;
    }
}