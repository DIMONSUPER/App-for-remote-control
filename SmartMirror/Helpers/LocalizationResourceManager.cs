using SmartMirror.Resources.Strings;
using System.Globalization;

namespace SmartMirror.Helpers
{
    public class LocalizationResourceManager
    {
        private LocalizationResourceManager()
        {
            Strings.Culture = CultureInfo.CurrentCulture;
        }

        #region -- Public properties --
        
        public static LocalizationResourceManager Instance { get; } = new();

        public object this[string resourceKey] => Strings.ResourceManager.GetObject(resourceKey, Strings.Culture) ?? resourceKey; 
        
        #endregion
    }
}
