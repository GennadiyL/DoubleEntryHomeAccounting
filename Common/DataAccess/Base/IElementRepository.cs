using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<T>
    where T : IEntity
{
    Task<List<T>> GetByName(Guid parentId, string name);
    Task<int> GetMaxOrder(Guid parentId);
    Task<int> GetCount(Guid parentId);
    Task<List<T>> GetList(Guid parentId);
}