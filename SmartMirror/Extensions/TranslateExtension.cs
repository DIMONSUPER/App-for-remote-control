using SmartMirror.Helpers;

namespace SmartMirror.Extensions
{
    [ContentProperty(nameof(Name))]
    public class TranslateExtension : IMarkupExtension
    {
        #region -- Public properties --

        public string? Name { get; set; }

        #endregion

        #region -- IMarkupExtension implementation --

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        #endregion

        #region -- Public methods --

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return LocalizationResourceManager.Instance[Name];
        }

        #endregion
    }
}