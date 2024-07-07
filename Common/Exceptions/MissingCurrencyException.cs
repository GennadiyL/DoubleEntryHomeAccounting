namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingCurrencyException : ApplicationBaseException
    {
        private const string InnerMessage = "Currency with iso code '{0}' does not exist in app";

        public string IsoCode { get; }

        public MissingCurrencyException(string isoCode) : base(isoCode, null)
        {
        }

        public MissingCurrencyException(string isoCode, Exception innerException) 
            : base(string.Format(InnerMessage, isoCode), innerException)
        {
            IsoCode = isoCode;
        }
    }
}
