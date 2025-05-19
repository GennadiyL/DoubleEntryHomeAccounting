using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class GroupCycleException : ApplicationBaseException
{
    private const string _innerMessage = "Found Cycle in the Group {0}.";
    
    public string TypeName { get; }
    
    public  GroupCycleException(Type entityType) : this(entityType, null)
    {
    }

    public GroupCycleException(Type entityType, Exception innerException) :
        base(string.Format(_innerMessage, entityType.Name), innerException)
    {
        TypeName = entityType.Name;
    }
}