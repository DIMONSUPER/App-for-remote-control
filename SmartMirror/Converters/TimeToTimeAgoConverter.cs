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
                    string[] words = { Strings.HourAgo, Strings.HoursAgo};

                    var index = GetIndexForNumber((int)differenceInTime.TotalHours);

                    result = $"{(int)differenceInTime.TotalHours} {words[index]}";
                }
                else if (differenceInTime.TotalMinutes >= 1)
                {
                    string[] words = { Strings.MinuteAgo, Strings.MinutesAgo};

                    var index = GetIndexForNumber((int)differenceInTime.TotalMinutes);

                    result = $"{(int)differenceInTime.TotalMinutes} {words[index]}";
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

        #region -- Private helpers --

        private int GetIndexForNumber(int number)
        {
            return number > 1 ? 1 : 0;
        }

        #endregion
    }
}
