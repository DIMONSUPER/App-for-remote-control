using SmartMirror.Helpers;
using System.Globalization;

namespace SmartMirror.Converters
{
    public class TimeToDaysOfWeekConverter : IValueConverter
    {
        #region -- IValueConverter implementation --

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (value is string dateToCompare)
            {
                var dateTimeNow = DateTime.Now.ToString(Constants.Formats.DATE_FORMAT);
                var dateTimeYesterday = DateTime.Now.AddDays(-1).ToString(Constants.Formats.DATE_FORMAT);

                result = dateToCompare switch
                {
                    _ when dateToCompare == dateTimeNow => (string)LocalizationResourceManager.Instance["Today"],
                    _ when dateToCompare == dateTimeYesterday => (string)LocalizationResourceManager.Instance["Yesterday"],
                    _ => dateToCompare,
                };
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
