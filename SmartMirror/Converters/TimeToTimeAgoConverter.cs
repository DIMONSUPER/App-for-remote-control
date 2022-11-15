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
                    var word = differenceInTime.TotalHours > 2 ? Strings.HoursAgo : Strings.HourAgo;

                    result = $"{(int)differenceInTime.TotalHours} {word}";
                }
                else if (differenceInTime.TotalMinutes >= 1)
                {
                    var word = differenceInTime.TotalMinutes > 2 ? Strings.MinutesAgo : Strings.MinuteAgo;

                    result = $"{(int)differenceInTime.TotalMinutes} {word}";
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
