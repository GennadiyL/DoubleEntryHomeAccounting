namespace GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

public interface INamedParam : IParam
{
    string Name { get; set; }
    string Description { get; set; }
}