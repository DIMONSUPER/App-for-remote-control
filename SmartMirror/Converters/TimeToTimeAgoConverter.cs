using SmartMirror.Helpers;
using SmartMirror.Resources.Strings;
using System.Globalization;

namespace SmartMirror.Converters
{
    public class TimeToTimeAgoConverter : IValueConverter
    {
        #region -- IValueConverter implementation --

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (value is DateTime timeToCompare)
            {
                var differenceInTime = DateTime.Now - timeToCompare;

                if (differenceInTime.TotalDays >= 1)
                {
                    result = timeToCompare.ToString(Constants.Formats.TIME_FORMAT);
                }
                else if (differenceInTime.TotalHours >= 1)
                {
                    result = $"{(int)differenceInTime.TotalHours} {Strings.HoursAgo}";
                }
                else if (differenceInTime.TotalMinutes >= 1)
                {
                    result = $"{(int)differenceInTime.TotalMinutes} {Strings.MinutesAgo}";
                }
                else
                {
                    result = Strings.Now;
                }
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
