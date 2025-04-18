namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

//TODO: REVIEW
//All Exceptions
public abstract class ApplicationBaseException : Exception
{
    protected ApplicationBaseException(string message) : base(message)
    {
    }

    protected ApplicationBaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}