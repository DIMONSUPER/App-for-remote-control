using System.Text;

namespace SmartMirror.Helpers;

public static class DateTimeHelper
{
    #region -- Public static helpers --

    public static long ConvertToMilliseconds(DateTime dateTime)
    {
        return (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    public static DateTime ConvertFromMilliseconds(long milliseconds)
    {
        return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(milliseconds);
    }

    public static string ConvertFromAqara(string time)
    {
        /*
          Input:
            time format: min hour day month week
            value: 1  1,2,3 * ?
        */
        string[] nameDayOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        var values = time.Split(' ');

        var day = values[2];
        var month = values[3];
        var week = values[4];

        var result = new StringBuilder($"{values[1].PadLeft(2, '0')}:{values[0].PadLeft(2, '0')} ");

        if (week is not ("1,2,3,4,5,6,0" or "*" or "?"))
        {
            if (week.Length == 1)
            {
                result.Append("every " + nameDayOfWeek[int.Parse(week)]);
            }
            else
            {
                result.Append("every " + week.Split(',')
                    .Select(row => nameDayOfWeek[int.Parse(row)])
                    .Aggregate((i, j) => i + ", " + j));
            }
        }
        else if (day == "*")
        {
            result.Append("every day");
        }
        else
        {
            if (day.Length > 2)
            {
                result.Append(day.Split(',').Aggregate((i, j) => i + ", " + j) + " days");
            }
            else
            {
                result.Append(day + " day");
            }
        }

        if (month == "*")
        {
            if (day != "*")
            {
                result.Append(" in every month");
            }
        }
        else
        {
            if (month.Length > 2)
            {
                result.Append(" in " + month.Split(',').Aggregate((i, j) => i + ", " + j) + " months");
            }
            else
            {
                result.Append(" in " + month + " month");
            }
        }
        
        return result.ToString();
    }

    #endregion
}

