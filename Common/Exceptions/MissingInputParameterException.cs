namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingInputParameterException : ApplicationBaseException
    {
        private const string InnerMessage = "Input parameter {0} cannot be null.";
        public string TypeName { get; }
        public MissingInputParameterException(string typeName) : this(typeName, null)
        {
        }

        public MissingInputParameterException(string typeName, Exception innerException)
            : base(string.Format(InnerMessage, typeName), innerException)
        {
            TypeName = typeName;
        }
    }
}
