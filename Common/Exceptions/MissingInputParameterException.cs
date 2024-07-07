namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingInputParameterException : ApplicationBaseException
    {
        private const string InnerMessage = "Input parameter {0} cannot be null.";
        public string TypeName { get; }
        public MissingInputParameterException(Type entityType) : this(entityType, null)
        {
        }

        public MissingInputParameterException(Type entityType, Exception innerException)
            : base(string.Format(InnerMessage, entityType.Name), innerException)
        {
            TypeName = entityType.Name;
        }
    }
}
