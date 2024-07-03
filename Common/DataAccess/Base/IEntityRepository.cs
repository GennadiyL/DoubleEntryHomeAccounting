using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IEntityRepository<T>
    where T : IEntity
{
    Task<T> GetById(
        Guid id, 
        Include<T> include = null);

    Task<T> GetFirstOrDefault(
        Func<T, bool> predicate = null, 
        Include<T> include = null, 
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null);

    Task<ICollection<T>> Where(
        Func<T, bool> predicate = null,
        Include<T> include = null,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null);

    Task<ICollection<T>> Where(
        int take,
        int skip,
        Func<T, bool> predicate = null,
        Include<T> include = null,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null);
    
    Task<ICollection<T>> GetAll();

    Task<int> GetCount<TP1>(
        Func<T, bool> predicate = null,
        Include<T> include = null);

    Task<T> Add(T entity);
    Task<IList<T>> Add(ICollection<T> entities);
    Task<T> Update(T entity);
    Task<IList<T>> Update(ICollection<T> entities);
    Task<T> Delete(Guid id);
    Task<IList<T>> Delete(ICollection<Guid> ids);
}