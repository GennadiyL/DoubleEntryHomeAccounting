using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IOrderedEntityService<T>
    where T : IOrderedEntity
{
    Task SetOrder(Guid entityId, int order);
}