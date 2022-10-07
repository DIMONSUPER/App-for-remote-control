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

    #endregion
}

