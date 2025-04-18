using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class MissingCurrencyException : ApplicationBaseException
{
    private const string _innerMessage = "Cannot find currency with iso code '{0}'.";

    public string IsoCode { get; }

    public MissingCurrencyException(string isoCode) : this(isoCode, null)
    {
    }

    public MissingCurrencyException(string isoCode, Exception innerException) 
        : base(string.Format(_innerMessage, isoCode), innerException)
    {
        IsoCode = isoCode;
    }
}