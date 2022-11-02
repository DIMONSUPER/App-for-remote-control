using System;
namespace SmartMirror.Services.Settings
{
    public class GoogleAccessSettings
    {
        #region -- Public properties --

        public string AccessToken
        {
            get => Preferences.Default.Get(nameof(AccessToken), default(string), nameof(GoogleAccessSettings));
            set => Preferences.Default.Set(nameof(AccessToken), value, nameof(GoogleAccessSettings));
        }

        public string RefreshToken
        {
            get => Preferences.Default.Get(nameof(RefreshToken), default(string), nameof(GoogleAccessSettings));
            set => Preferences.Default.Set(nameof(RefreshToken), value, nameof(GoogleAccessSettings));
        }

        #endregion

        #region -- Public helpers --

        public void SetAccessSettings(GoogleAccessSettings accessResponse)
        {
            AccessToken = accessResponse.AccessToken;
            RefreshToken = accessResponse.RefreshToken;
        }

        public void Clear()
        {
            try
            {
                Preferences.Default.Clear(nameof(GoogleAccessSettings));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(Clear)} in {nameof(GoogleAccessSettings)}: {ex.Message}");
            }
        }

        #endregion
    }
}

