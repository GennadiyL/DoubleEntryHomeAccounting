using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class MismatchingCurrenciesException : ApplicationBaseException
{
    private const string _innerMessage = "Can't combine entities with different currencies";

    public MismatchingCurrenciesException() : this(null)
    {
    }

    public MismatchingCurrenciesException(Exception innerException) : base(_innerMessage, innerException)
    {
    }
}