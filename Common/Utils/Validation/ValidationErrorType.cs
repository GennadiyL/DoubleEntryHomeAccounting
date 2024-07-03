namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Validation;

public enum ValidationErrorType
{
    None = 0,
    MissedId,
    DuplicatedId,
    EmptyName,
    DuplicatedName,
    WrongOrders
}