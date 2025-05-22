using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class ReadonlyRootGroupException : ApplicationBaseException
{
    private const string _innerMessage = "Root Group {0} must be readonly.";

    public string TypeName { get; }

    public ReadonlyRootGroupException(Type entityType) : this(entityType, null)
    {
    }

    public ReadonlyRootGroupException(Type entityType, Exception innerException) :
        base(string.Format(_innerMessage, entityType.Name), innerException)
    {
        TypeName = entityType.Name;
    }
}