using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IGroupRepository<T>
    where T : IEntity
{
    Task <List<T>> GetByName(string name);
    Task<int> GetMaxOrder();
    Task<int> GetCount();
    Task<List<T>> GetList();
}