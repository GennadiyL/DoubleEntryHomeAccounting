using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

// ReSharper disable once UnusedTypeParameter
public interface IOrderedService<T>
    where T : IOrderedEntity
{
    Task SetOrder(Guid entityId, int order);
}