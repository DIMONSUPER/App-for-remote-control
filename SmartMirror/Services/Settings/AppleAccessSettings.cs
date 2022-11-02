using System;
using SmartMirror.Models.Aqara;

namespace SmartMirror.Services.Settings
{
    public class AppleAccessSettings
    {
        #region -- Public properties --

        public string AccessToken
        {
            get => Preferences.Default.Get(nameof(AccessToken), default(string), nameof(AppleAccessSettings));
            set => Preferences.Default.Set(nameof(AccessToken), value, nameof(AppleAccessSettings));
        }

        public string RefreshToken
        {
            get => Preferences.Default.Get(nameof(RefreshToken), default(string), nameof(AppleAccessSettings));
            set => Preferences.Default.Set(nameof(RefreshToken), value, nameof(AppleAccessSettings));
        }

        #endregion

        #region -- Public helpers --

        public void SetAccessSettings(AppleAccessSettings accessResponse)
        {
            AccessToken = accessResponse.AccessToken;
            RefreshToken = accessResponse.RefreshToken;
        }

        public void Clear()
        {
            try
            {
                Preferences.Default.Clear(nameof(AppleAccessSettings));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(Clear)} in {nameof(AppleAccessSettings)}: {ex.Message}");
            }
        }

        #endregion
    }
}

