﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IEntityRepository<T> : IEntityRepository
    where T : IEntity
{
    Task<T> GetById(Guid id);
    Task<ICollection<T>> GetAll();
    Task<int> GetCount<TP1>();

    Task<T> Add(T entity);
    Task<IList<T>> Add(ICollection<T> entities);
    Task<T> Update(T entity);
    Task<IList<T>> Update(ICollection<T> entities);
    Task<T> Delete(Guid id);
    Task<IList<T>> Delete(ICollection<Guid> ids);
}