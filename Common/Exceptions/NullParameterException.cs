using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class NullParameterException : ApplicationBaseException
{
    private const string _innerMessage = "Parameter {0} cannot be null.";
    
    public string TypeName { get; }
    
    public NullParameterException(Type entityType) : this(entityType, null)
    {
    }

    public NullParameterException(Type entityType, Exception innerException) :
        base(string.Format(_innerMessage, entityType.Name), innerException)
    {
        TypeName = entityType.Name;
    }
}