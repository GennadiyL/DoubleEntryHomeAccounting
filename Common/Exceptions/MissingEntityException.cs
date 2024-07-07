namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class MissingEntityException : ApplicationBaseException
    {
        private const string InnerMessage = "Cannot find {0} entity with id {1}.";
        public string TypeName { get; }
        public Guid Id { get; }

        public MissingEntityException(string typeName, Guid id) : this(typeName, id, null)
        {
        }

        public MissingEntityException(string typeName, Guid id, Exception innerException) 
            : base(string.Format(InnerMessage, typeName, id), innerException)
        {
            TypeName = typeName;
            Id = id;
        }
    }
}
