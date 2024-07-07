namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class InvalidCurrencyIsoCodeException : ApplicationBaseException
    {
        private const string InnerMessage = "Invalid currency iso code '{0}'.";

        public string IsoCode { get; }

        public InvalidCurrencyIsoCodeException(string isoCode) : this(isoCode, null)
        {
        }

        public InvalidCurrencyIsoCodeException(string isoCode, Exception innerException)
            : base(string.Format(InnerMessage, isoCode), innerException)
        {
            IsoCode = isoCode;
        }
    }
}
