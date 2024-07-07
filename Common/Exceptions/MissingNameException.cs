namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingNameException : ApplicationBaseException
    {
        private const string InnerMessage = "Name {0} cannot be null.";
        public string TypeName { get; }
        public MissingNameException(Type entityType) : this(entityType, null)
        {
        }

        public MissingNameException(Type entityType, Exception innerException)
            : base(string.Format(InnerMessage, entityType.Name), innerException)
        {
            TypeName = entityType.Name;
        }
    }
}
