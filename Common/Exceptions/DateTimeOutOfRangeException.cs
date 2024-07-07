namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class DateTimeOutOfRangeException :  ApplicationBaseException
    {
        private const string InnerMessage = "Date or DateTime should be between {0} and {1}. But now it is {2}.";

        public DateOnly MinDate { get; }
        public DateOnly MaxDate { get; }
        public DateTime Current { get; }

        public DateTimeOutOfRangeException(DateOnly minDate, DateOnly maxDate, DateTime current) 
            : this(minDate, maxDate, current, null)
        {
        }

        public DateTimeOutOfRangeException(DateOnly minDate, DateOnly maxDate, DateOnly current) 
            : this(minDate, maxDate, current, null)
        {
        }

        public DateTimeOutOfRangeException(DateOnly minDate, DateOnly maxDate, DateTime current, Exception innerException) 
            : base(string.Format(InnerMessage, minDate, maxDate, current), innerException)
        {
            MinDate = minDate;
            MaxDate = maxDate;
            Current = current;
        }
        public DateTimeOutOfRangeException(DateOnly minDate, DateOnly maxDate, DateOnly current, Exception innerException)
            : base(string.Format(InnerMessage, minDate, maxDate, current), innerException)
        {
            MinDate = minDate;
            MaxDate = maxDate;
            Current = new DateTime(current.Year, current.Month, current.Day);
        }
    }
}
