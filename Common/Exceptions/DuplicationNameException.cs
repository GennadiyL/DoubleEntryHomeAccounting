using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class DuplicationNameException : ApplicationBaseException
{
    private const string _innerMessage = "{0} entity with name {1} already exists.";

    public string TypeName { get; }
    public string Name { get; }

    public DuplicationNameException(Type entityType, string name) : this(entityType, name, null)
    {
    }

    public DuplicationNameException(Type entityType, string name, Exception innerException) : 
        base(string.Format(_innerMessage, entityType.Name, name), innerException)
    {
        TypeName = entityType.Name;
        Name = name;
    }
}