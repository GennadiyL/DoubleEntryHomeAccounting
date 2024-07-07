namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MismatchingCurrenciesException : ApplicationBaseException
    {
        private const string InnerMessage = "Can't combine entities with different currencies";

        public MismatchingCurrenciesException() : this(null)
        {
        }

        public MismatchingCurrenciesException(Exception innerException) : base(InnerMessage, innerException)
        {
        }
    }
}
