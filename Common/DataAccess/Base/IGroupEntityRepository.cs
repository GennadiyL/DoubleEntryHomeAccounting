﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IGroupEntityRepository<TGroup, TElement> : IEntityRepository<TGroup>, IGroupRepository<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
}