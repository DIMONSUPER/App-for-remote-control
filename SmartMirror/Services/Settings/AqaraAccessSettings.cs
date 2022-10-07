using System;
using SmartMirror.Helpers;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Settings;

public class AqaraAccessSettings
{
    #region -- Public properties --

    public string AccessToken
    {
        get => Preferences.Default.Get(nameof(AccessToken), default(string), nameof(AqaraAccessSettings));
        set => Preferences.Default.Set(nameof(AccessToken), value, nameof(AqaraAccessSettings));
    }

    public string RefreshToken
    {
        get => Preferences.Default.Get(nameof(RefreshToken), default(string), nameof(AqaraAccessSettings));
        set => Preferences.Default.Set(nameof(RefreshToken), value, nameof(AqaraAccessSettings));
    }

    public string OpenId
    {
        get => Preferences.Default.Get(nameof(OpenId), default(string), nameof(AqaraAccessSettings));
        set => Preferences.Default.Set(nameof(OpenId), value, nameof(AqaraAccessSettings));
    }

    public DateTime ExpiresAt
    {
        get => DateTimeHelper.ConvertFromMilliseconds(Preferences.Default.Get<long>(nameof(ExpiresAt), 0, nameof(AqaraAccessSettings)));
        set => Preferences.Default.Set(nameof(ExpiresAt), DateTimeHelper.ConvertToMilliseconds(value), nameof(AqaraAccessSettings));
    }

    #endregion

    #region -- Public helpers --

    public void SetAccessSettings(AccessResponse accessResponse)
    {
        AccessToken = accessResponse.AccessToken;
        RefreshToken = accessResponse.RefreshToken;
        OpenId = accessResponse.OpenId;
        ExpiresAt = DateTime.UtcNow.AddSeconds(accessResponse.ExpiresIn);
    }

    public void Clear()
    {
        try
        {
            Preferences.Default.Clear(nameof(AqaraAccessSettings));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{nameof(Clear)} in {nameof(AqaraAccessSettings)}: {ex.Message}");
        }
    }

    #endregion

    #region -- Private helpers --


    #endregion
}

