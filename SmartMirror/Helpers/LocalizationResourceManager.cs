using System;
using System.ComponentModel;
using System.Globalization;
using SmartMirror.Resources.Strings;

namespace SmartMirror.Helpers
{
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        public static LocalizationResourceManager Instance { get; } = new();

        private LocalizationResourceManager()
        {
            Strings.Culture = CultureInfo.CurrentCulture;
        }

        public object this[string resourceKey] => Strings.ResourceManager.GetObject(resourceKey, Strings.Culture) ?? Array.Empty<byte>();

        #region -- INotifyPropertyChanged implementation --

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region -- Public methods -- 

        public void SetCulture(CultureInfo culture)
        {
            Strings.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        #endregion
    }
}

