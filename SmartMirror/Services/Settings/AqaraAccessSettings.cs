using System;
namespace SmartMirror.Services.Settings
{
    public class AqaraAccessSettings
    {
        #region -- Public properties --

        public string AccessToken
        {
            get => Preferences.Get(nameof(AccessToken), default(string));
            set => Preferences.Set(nameof(AccessToken), value, nameof(AqaraAccessSettings));
        }

        public string RefreshToken
        {
            get => Preferences.Get(nameof(RefreshToken), default(string));
            set => Preferences.Set(nameof(RefreshToken), value, nameof(AqaraAccessSettings));
        }

        public string OpenId
        {
            get => Preferences.Get(nameof(OpenId), default(string));
            set => Preferences.Set(nameof(OpenId), value, nameof(AqaraAccessSettings));
        }

        public DateTime ExpiresAt
        {
            get => Preferences.Get(nameof(ExpiresAt), DateTime.MinValue);
            set => Preferences.Set(nameof(ExpiresAt), value, nameof(AqaraAccessSettings));
        }

        #endregion

        #region -- Public helpers --

        public void Clear()
        {
            try
            {
                Preferences.Clear(nameof(AqaraAccessSettings));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(Clear)} in {nameof(AqaraAccessSettings)}: {ex.Message}");
            }
        }

        #endregion
    }
}

