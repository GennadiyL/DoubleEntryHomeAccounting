namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class ReferenceEntityException : ApplicationBaseException
    {
        private const string InnerMessage = "{0} entity with id {2} cannot be deleted.  One or more {1} entites use it.";
        public string ParentTypeName { get; }
        public string ChildTypeName { get; }
        public Guid EntityId { get; }

        public ReferenceEntityException(Type parentType, Type childType, Guid entityId) 
            : this(parentType, childType, entityId, null)
        {
        }

        public ReferenceEntityException(Type parentType, Type childType, Guid entityId, Exception innerException) 
            : base(string.Format(InnerMessage, parentType.Name, childType.Name, entityId), innerException)
        {
            ParentTypeName = parentType.Name;
            ChildTypeName = childType.Name;
            EntityId = entityId;
        }
    }
}
