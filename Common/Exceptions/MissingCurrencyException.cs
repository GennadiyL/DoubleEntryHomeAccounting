namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingCurrencyException : ApplicationBaseException
    {
        private const string InnerMessage = "Cannot find currency with iso code '{0}'.";

        public string IsoCode { get; }

        public MissingCurrencyException(string isoCode) : this(isoCode, null)
        {
        }

        public MissingCurrencyException(string isoCode, Exception innerException) 
            : base(string.Format(InnerMessage, isoCode), innerException)
        {
            IsoCode = isoCode;
        }
    }
}
