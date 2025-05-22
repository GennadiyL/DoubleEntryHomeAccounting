using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class NullNameException : ApplicationBaseException
{
    private const string _innerMessage = "Name {0} cannot be null.";

    public string TypeName { get; }

    public NullNameException(Type entityType) : this(entityType, null)
    {
    }

    public NullNameException(Type entityType, Exception innerException) :
        base(string.Format(_innerMessage, entityType.Name), innerException)
    {
        TypeName = entityType.Name;
    }
}