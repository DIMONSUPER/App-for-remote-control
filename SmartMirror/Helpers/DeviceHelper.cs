using SmartMirror.Models.BindableModels;
using SmartMirror.Resources;

namespace SmartMirror.Helpers;

public static class DeviceHelper
{
    #region -- Public static helpers --

    public static string GetImageSourceForWeather(DeviceBindableModel device)
    {
        var result = IconsNames.grey_question_mark;

        if (device.EditableResourceId is not null)
        {
            result = device.EditableResourceId switch
            {
                Constants.Aqara.AttibutesId.HUMIDITY_STATUS => IconsNames.pic_humidity,
                Constants.Aqara.AttibutesId.TEMPERATURE_STATUS => IconsNames.pic_temperature,
                Constants.Aqara.AttibutesId.AIR_PRESSURE_STATUS => IconsNames.pic_pressure,
                _ => IconsNames.grey_question_mark,
            };
        }
        else
        {
            result = device.Model switch
            {
                Constants.Aqara.Models.LUMI_WEATHER_V1 => IconsNames.pic_temperature,
                _ => IconsNames.grey_question_mark,
            };
        }

        return result;
    }

    public static string GetImageSourceForMotionSensor(DeviceBindableModel device)
    {
        var result = device.EditableResourceId switch
        {
            Constants.Aqara.AttibutesId.MOTION_STATUS => IconsNames.pic_motion,
            Constants.Aqara.AttibutesId.LUX_STATUS => IconsNames.pic_dimmer,
            _ => IconsNames.pic_motion,
        };

        return result;
    }

    public static string GetImageSourceForHub(DeviceBindableModel device)
    {
        return IconsNames.pic_hub;
    }

    public static bool IsDeviceSwitch(DeviceBindableModel device)
    {
        return device.Model.Contains("switch");
    }

    public static bool IsDeviceWeather(DeviceBindableModel device)
    {
        return device.Model.Contains("weather");
    }

    public static bool IsDeviceMotionSensor(DeviceBindableModel device)
    {
        return device.Model.Contains("motion");
    }

    public static bool IsDeviceHub(DeviceBindableModel device)
    {
        return device.Model.Contains("gateway");
    }

    #endregion
}

