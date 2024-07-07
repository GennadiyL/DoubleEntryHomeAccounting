namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public abstract class ApplicationBaseException : Exception
    {
        protected ApplicationBaseException(string message) : base(message)
        {
        }

        protected ApplicationBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
