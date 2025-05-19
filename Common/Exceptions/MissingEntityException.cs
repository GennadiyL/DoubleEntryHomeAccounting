using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class MissingEntityException : ApplicationBaseException
{
    private const string _innerMessage = "Cannot find {0} entity with id {1}.";
    
    public string TypeName { get; }
    public Guid Id { get; }

    public MissingEntityException(Type entityType, Guid id) : this(entityType, id, null)
    {
    }

    public MissingEntityException(Type entityType, Guid id, Exception innerException) :
        base(string.Format(_innerMessage, entityType.Name, id), innerException)
    {
        TypeName = entityType.Name;
        Id = id;
    }
}