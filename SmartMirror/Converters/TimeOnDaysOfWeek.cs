using SmartMirror.Helpers;
using System.Globalization;

namespace SmartMirror.Converters
{
    public class TimeOnDaysOfWeek : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (value is string dateToCompare)
            {
                var dateTimeNow = DateTime.Now.ToString(Constants.Formats.DATE_FORMAT);
                var dateTimeYesterday = DateTime.Now.AddDays(-1).ToString(Constants.Formats.DATE_FORMAT);

                string TitleSelector(string dateTime) => dateTime switch
                {
                    _ when dateTime == dateTimeNow => (string)LocalizationResourceManager.Instance["Today"],
                    _ when dateTime == dateTimeYesterday => (string)LocalizationResourceManager.Instance["Yesterday"],
                    _ => dateTime,
                };

                result = TitleSelector(dateToCompare);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
