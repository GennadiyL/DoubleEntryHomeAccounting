namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IOrderedEntity : IEntity
{
    int Order { get; set; }
}