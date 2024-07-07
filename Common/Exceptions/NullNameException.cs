namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class NullNameException : ApplicationBaseException
    {
        private const string InnerMessage = "Name {0} cannot be null.";
        public string TypeName { get; }
        public NullNameException(Type entityType) : this(entityType, null)
        {
        }

        public NullNameException(Type entityType, Exception innerException)
            : base(string.Format(InnerMessage, entityType.Name), innerException)
        {
            TypeName = entityType.Name;
        }
    }
}
