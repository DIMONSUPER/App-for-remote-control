using System;
using System.Globalization;

namespace SmartMirror.Converters
{
    public class TextToTruncatedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;

            if (!string.IsNullOrEmpty(text) && parameter is int length && length > 0 && length < text.Length)
            {
                text = $"{text.Substring(0, length)}...";
            }

            return text ?? value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

