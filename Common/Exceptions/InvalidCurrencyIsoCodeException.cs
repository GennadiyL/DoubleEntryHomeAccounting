using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class InvalidCurrencyIsoCodeException : ApplicationBaseException
{
    private const string _innerMessage = "Invalid currency iso code '{0}'.";

    public string IsoCode { get; }

    public InvalidCurrencyIsoCodeException(string isoCode) : this(isoCode, null)
    {
    }

    public InvalidCurrencyIsoCodeException(string isoCode, Exception innerException) :
        base(string.Format(_innerMessage, isoCode), innerException)
    {
        IsoCode = isoCode;
    }
}