using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IEntityRepository<T>
    where T : IEntity
{
    #region GetById
    Task<T> GetById(Guid id);
    Task<T> GetById<TP1>(
        Guid id, 
        Include<T, TP1> include1)
        where TP1 : IEntity;
    Task<T> GetById<TP1, TP2>(
        Guid id, 
        Include<T, TP1> include1, 
        Include<T, TP2> include2)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<T> GetById<TP1, TP2, TP3, TP4>(
        Guid id, 
        Include<T, TP1> include1, 
        Include<T, TP2> include2, 
        Include<T, TP3> include3,
        Include<T, TP3> include4)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region GetFirstOrDefault
    Task<T> GetFirstOrDefault(
        Func<T, bool> predicate = null, 
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null);
    Task<T> GetFirstOrDefault<TP1>(
        Func<T, bool> predicate, 
        Include<T, TP1> include1, 
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity;
    Task<T> GetFirstOrDefault<TP1, TP2>(
        Func<T, bool> predicate, 
        Include<T, TP1> include1, 
        Include<T, TP2> include2, 
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<T> GetFirstOrDefault<TP1, TP2, TP3>(
        Func<T, bool> predicate, 
        Include<T, TP1> include1, 
        Include<T, TP2> include2, 
        Include<T, TP3> include3, 
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity;
    Task<T> GetFirstOrDefault<TP1, TP2, TP3, TP4>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3,
        Include<T, TP4> include4,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region Where
    Task<ICollection<T>> Where(
        Func<T, bool> predicate = null,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null);
    Task<ICollection<T>> Where<TP1>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity;
    Task<ICollection<T>> Where<TP1, TP2>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<ICollection<T>> Where<TP1, TP2, TP3>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity;
    Task<ICollection<T>> Where<TP1, TP2, TP3, TP4>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3,
        Include<T, TP4> include4,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region Where (Page)

    Task<ICollection<T>> Where(
        int take, 
        int skip,
        Func<T, bool> predicate = null,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null);
    Task<ICollection<T>> Where<TP1>(
        int take,
        int skip,
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity;
    Task<ICollection<T>> Where<TP1, TP2>(
        int take,
        int skip,
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<ICollection<T>> Where<TP1, TP2, TP3>(
        int take,
        int skip,
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity;
    Task<ICollection<T>> Where<TP1, TP2, TP3, TP4>(
        int take,
        int skip,
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3,
        Include<T, TP4> include4,
        Func<ICollection<T>, IOrderedEnumerable<T>> orderBy = null)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region GetAll

    Task<IEnumerable<T>> GetAll();

    #endregion

    #region GetCount
    Task<int> GetCount(
        Func<T, bool> predicate = null);
    Task<int> GetCount<TP1>(
        Func<T, bool> predicate,
        Include<T, TP1> include1)
        where TP1 : IEntity;
    Task<int> GetCount<TP1, TP2>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<int> GetCount<TP1, TP2, TP3>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity;
    Task<int> GetCount<TP1, TP2, TP3, TP4>(
        Func<T, bool> predicate,
        Include<T, TP1> include1,
        Include<T, TP2> include2,
        Include<T, TP3> include3,
        Include<T, TP4> include4)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region CRUD

    Task<T> Save(T entity);
    Task<IList<T>> SaveList(IList<T> list);
    Task<T> Delete(Guid id);
    Task<IList<T>> DeleteList(IList<Guid> ids);

    #endregion
}