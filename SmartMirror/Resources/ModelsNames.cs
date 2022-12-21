namespace SmartMirror.Resources;

public static class ModelsNames
{
    private static Dictionary<string, string> _models { get; set; } = new()
    {
        { Constants.Aqara.Models.APP_TIMER_V1, "Timer" },
        { Constants.Aqara.Models.APP_IFTTT_V1, "If" },
        { Constants.Aqara.Models.APP_IFTTT_POSITION_USER_ALERT, "When the alarm sounds" },
        { Constants.Aqara.Models.APP_GEOFENCE_TRIGGER, "Arrive at a position" },
        { Constants.Aqara.Models.APP_WEATHER_HUMIDITY, "Weather humidity" },
        { Constants.Aqara.Models.APP_WEATHER_SUN, "Weather sun" },
        { Constants.Aqara.Models.APP_WEATHER_TEMPERATURE, "Weather temperature" },
        { Constants.Aqara.Models.APP_WEATHER_PHENOMENON, "Weather phenomenon" },
        { Constants.Aqara.Models.APP_WEATHER_FORECAST, "Weather forecast" },
        { Constants.Aqara.Models.APP_GEOFENCE_FORECAST, "Geofence forecast" },
        { Constants.Aqara.Models.APP_SCENE_V1, "Scene" },
        { Constants.Aqara.Models.APP_MOBILEPUSH_V1, "Push notification" },
    };

    public static string GetName(string model)
    {
        return _models.ContainsKey(model)
            ? _models[model]
            : model;
    }
}