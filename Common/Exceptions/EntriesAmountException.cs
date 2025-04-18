namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class EntriesAmountException : ApplicationBaseException
{
    private const string InnerMessage = "Amount of entries in the transaction and template must be 2 or more.";

    public EntriesAmountException() : this(null)
    {
    }

    public EntriesAmountException(Exception innerException)
        : base(InnerMessage, innerException)
    {
    }
}