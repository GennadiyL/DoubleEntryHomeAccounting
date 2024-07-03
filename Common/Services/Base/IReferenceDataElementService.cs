﻿using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceDataElementService<TGroup, TElement, in TParam> : IReferenceDataService<TElement, TParam>, IElementService<TGroup, TElement>
    where TGroup : IEntity, IFavoriteEntity, IOrderedEntity, IGroupEntity<TGroup, TElement>
    where TElement : IEntity, IFavoriteEntity, IOrderedEntity, IElementEntity<TGroup>
    where TParam : IParam
{
}