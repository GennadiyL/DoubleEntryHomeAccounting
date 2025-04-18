namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class DuplicationNameException : ApplicationBaseException
{
    private const string InnerMessage = "{0} entity with name {1} already exists.";

    public string TypeName { get; }
    public string Name { get; }

    public DuplicationNameException(Type entityType, string name) : this(entityType, name, null)
    {
    }

    public DuplicationNameException(Type entityType, string name, Exception innerException)
        : base(string.Format(InnerMessage, entityType.Name, name), innerException)
    {
        TypeName = entityType.Name;
        Name = name;
    }
}