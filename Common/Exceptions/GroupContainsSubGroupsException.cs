using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class GroupContainsSubGroupsException : ApplicationBaseException
{
    private const string _innerMessage = "{0}Group contains {1} {0}SubGroups and cannot be deleted.";

    public string TypeName { get; }
    public int SubGroupAmount { get; }

    public GroupContainsSubGroupsException(Type entityType, int subGroupAmount) : this(entityType, subGroupAmount, null)
    {
    }

    public GroupContainsSubGroupsException(Type entityType, int subGroupAmount, Exception innerException) :
        base(string.Format(_innerMessage, entityType.Name, subGroupAmount), innerException)
    {
        TypeName = entityType.Name;
        SubGroupAmount = subGroupAmount;
    }
}