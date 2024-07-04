﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IGroupRepository<TGroup, TElement>
    where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : IReferenceDataElementEntity<TGroup>
{
    Task<TGroup> GetByParentId(Guid? parentId);
    Task<ICollection<TGroup>> GetByParentIdAndName(Guid? parentId, string name);

    Task<int> GetMaxOrder(Guid? parentId);
    Task<int> GetCount(Guid? parentId);
}