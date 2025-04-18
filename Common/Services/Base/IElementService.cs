﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IElementService<TGroup, TElement, in TParam> : IGeneralService<TElement, TParam>
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement : class, IElementReferenceEntity<TGroup, TElement>, IReferenceEntity
    where TParam : class, IParam
{
    Task MoveToAnotherGroup(Guid entityId, Guid groupId);
    Task CombineElements(Guid primaryId, Guid secondaryId);
}