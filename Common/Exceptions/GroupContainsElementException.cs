namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class GroupContainsElementException : ApplicationBaseException
{
    private const string InnerMessage = "{0}Group contains {1} {0} elements and cannot be deleted.";
    public string TypeName { get; }
    public int ChildrenAmount { get; }

    public GroupContainsElementException(Type entityType, int childrenAmount) : this(entityType, childrenAmount, null)
    {
    }

    public GroupContainsElementException(Type entityType, int childrenAmount, Exception innerException) 
        : base(string.Format(InnerMessage, entityType.Name, childrenAmount), innerException)
    {
        TypeName = entityType.Name;
        ChildrenAmount = childrenAmount;
    }
}