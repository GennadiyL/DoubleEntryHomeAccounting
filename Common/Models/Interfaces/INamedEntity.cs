namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface INamedEntity : IEntity
{
    string Name { get; set; }
    string Description { get; set; }
}