using System.Globalization;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Convertors;

public static class DateTimeConvertor
{
    public const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.SSS";

    public static string ConvertToString(DateTime dateTime)
    {
        return dateTime.ToString(DateTimeFormat);
    }

    public static string ConvertToString()
    {
        DateTime dateTime = DateTime.UtcNow;
        return ConvertToString(dateTime);
    }

    public static DateTime ConvertFromString(string stamp)
    {
        return DateTime.ParseExact(stamp, DateTimeFormat, CultureInfo.InvariantCulture);
    }
}