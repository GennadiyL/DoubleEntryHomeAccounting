using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<TGroup, TElement>
    where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : IReferenceDataElementEntity<TGroup>
{
    #region GetByName
    Task<ICollection<TElement>> GetByName(
        Guid groupId, 
        string name);
    Task<ICollection<TElement>> GetByName<TP1>(
        Guid groupId,
        string name,
        Func<TElement, TP1> include1)
        where TP1 : IEntity;
    Task<ICollection<TElement>> GetByName<TP1, TP2>(
        Guid groupId,
        string name,
        Func<TElement, TP1> include1,
        Func<TElement, TP2> include2)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<ICollection<TElement>> GetByName<TP1, TP2, TP3>(
        Guid groupId,
        string name,
        Func<TElement, TP1> include1,
        Func<TElement, TP2> include2,
        Func<TElement, TP3> include3)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity;
    Task<ICollection<TElement>> GetByName<TP1, TP2, TP3, TP4>(
        Guid groupId,
        string name,
        Func<TElement, bool> predicate,
        Func<TElement, TP1> include1,
        Func<TElement, TP2> include2,
        Func<TElement, TP3> include3,
        Func<TElement, TP4> include4)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region GetElements
    Task<ICollection<TElement>> GetElements(
        Guid groupId);
    Task<ICollection<TElement>> GetElements<TP1>(
        Guid groupId,
        Func<TElement, TP1> include1)
        where TP1 : IEntity;
    Task<ICollection<TElement>> GetElements<TP1, TP2>(
        Guid groupId,
        Func<TElement, TP1> include1,
        Func<TElement, TP2> include2)
        where TP1 : IEntity
        where TP2 : IEntity;
    Task<ICollection<TElement>> GetElements<TP1, TP2, TP3>(
        Guid groupId,
        Func<TElement, TP1> include1,
        Func<TElement, TP2> include2,
        Func<TElement, TP3> include3)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity;
    Task<ICollection<TElement>> GetElements<TP1, TP2, TP3, TP4>(
        Guid groupId,
        Func<TElement, bool> predicate,
        Func<TElement, TP1> include1,
        Func<TElement, TP2> include2,
        Func<TElement, TP3> include3,
        Func<TElement, TP4> include4)
        where TP1 : IEntity
        where TP2 : IEntity
        where TP3 : IEntity
        where TP4 : IEntity;
    #endregion

    #region Misk
    Task<int> GetMaxOrder(Guid groupId);
    Task<int> GetCount(Guid groupId);
    #endregion
}