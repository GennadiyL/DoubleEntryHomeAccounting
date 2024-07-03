namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Validation;

public class ValidationError
{
    public ValidationError(ValidationErrorType type, Guid id, string entityTypeName)
    {
        Type = type;
        Id = id;
        EntityTypeName = entityTypeName;
    }

    public ValidationErrorType Type { get; }

    public Guid Id { get; }

    public string EntityTypeName { get; }
}