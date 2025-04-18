
namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;

public class InvalidEnumerationException : ApplicationBaseException
{
    private const string InnerMessage = "{0} doesn`t contain value {1}.";
    public string TypeName { get; }
    public ValueType Value { get; }

    public InvalidEnumerationException(Type type, ValueType value) : this(type, value, null)
    {
    }

    public InvalidEnumerationException(Type type, ValueType value, Exception innerException) 
        : base(string.Format(InnerMessage, type.Name, value), innerException)
    {
        TypeName = type.Name;
        Value = value;
    }
}