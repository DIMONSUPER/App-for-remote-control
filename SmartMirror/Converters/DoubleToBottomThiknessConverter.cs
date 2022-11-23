using System.Globalization;

namespace SmartMirror.Converters;

public class DoubleToBottomThiknessConverter : IValueConverter
{
    #region -- IValueConverter implementation -

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Thickness result;

        if (value is double dValue)
        {
            result = new(0, 0, 0, dValue);
        }
        else
        {
            result = new();
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    #endregion
}

