using System.Globalization;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Convertors
{
    public static class DateTimeConvertor
    {
        public const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.SSS";

        public static string ConvertToString(System.DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormat);
        }

        public static string ConvertToString()
        {
            System.DateTime dateTime = System.DateTime.UtcNow;
            return ConvertToString(dateTime);
        }

        public static System.DateTime ConvertFromString(string stamp)
        {
            return System.DateTime.ParseExact(stamp, DateTimeFormat, CultureInfo.InvariantCulture);
        }
    }
}
