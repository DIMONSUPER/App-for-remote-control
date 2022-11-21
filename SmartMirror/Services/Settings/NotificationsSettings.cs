using System;

namespace SmartMirror.Services.Settings
{
	public class NotificationsSettings
	{
        #region -- Public properties --

        public bool IsAllowNotifications
        {
            get => Preferences.Default.Get(nameof(IsAllowNotifications), true, nameof(NotificationsSettings));
            set => Preferences.Default.Set(nameof(IsAllowNotifications), value, nameof(NotificationsSettings));
        }

        #endregion
    }
}
