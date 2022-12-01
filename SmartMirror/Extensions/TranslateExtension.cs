using SmartMirror.Helpers;

namespace SmartMirror.Extensions
{
    [ContentProperty(nameof(Name))]
    public class TranslateExtension : IMarkupExtension
    {
        #region -- Public properties --

        public string Name { get; set; }

        public string StringFormat { get; set; }

        #endregion

        #region -- IMarkupExtension implementation --

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        #endregion

        #region -- Public helpers --

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var text = LocalizationResourceManager.Instance[Name];

            var formattedText = TryFormat(StringFormat, text);

            return formattedText == string.Empty
                ? text
                : formattedText;
        }

        #endregion

        #region -- Private helpers --

        private string TryFormat(string format, object value)
        {
            var result = string.Empty;

            try
            {
                if (format is not null)
                {
                    result = string.Format(format, value); 
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(TryFormat)}: {ex.Message}");
            }

            return result;
        }

        #endregion
    }
}