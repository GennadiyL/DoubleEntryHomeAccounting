namespace GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

public interface IGroupParam : IParam
{
    Guid? ParentId { get; set; }
}