using System;
namespace SmartMirror.Services.Settings
{
    public class AmazonAccessSettings
    {
        #region -- Public properties --

        public string AccessToken
        {
            get => Preferences.Default.Get(nameof(AccessToken), default(string), nameof(AmazonAccessSettings));
            set => Preferences.Default.Set(nameof(AccessToken), value, nameof(AmazonAccessSettings));
        }

        public string RefreshToken
        {
            get => Preferences.Default.Get(nameof(RefreshToken), default(string), nameof(AmazonAccessSettings));
            set => Preferences.Default.Set(nameof(RefreshToken), value, nameof(AmazonAccessSettings));
        }

        #endregion

        #region -- Public helpers --

        public void SetAccessSettings(AmazonAccessSettings accessResponse)
        {
            AccessToken = accessResponse.AccessToken;
            RefreshToken = accessResponse.RefreshToken;
        }

        public void Clear()
        {
            try
            {
                Preferences.Default.Clear(nameof(AmazonAccessSettings));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(Clear)} in {nameof(AmazonAccessSettings)}: {ex.Message}");
            }
        }

        #endregion

    }
}

