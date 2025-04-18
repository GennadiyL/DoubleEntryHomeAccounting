using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class EntriesAmountException : ApplicationBaseException
{
    private const string _innerMessage = "Amount of entries in the transaction and template must be 2 or more.";

    public EntriesAmountException() : this(null)
    {
    }

    public EntriesAmountException(Exception innerException)
        : base(_innerMessage, innerException)
    {
    }
}