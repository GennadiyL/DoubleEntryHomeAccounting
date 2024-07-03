using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;


public interface IReferenceDataGroupService<TGroup, TElement, in TParam> : IReferenceDataService<TElement, TParam>, IGroupService<TGroup, TElement>
    where TGroup : IEntity, IFavoriteEntity, IOrderedEntity, IGroupEntity<TGroup, TElement>
    where TElement : IEntity, IFavoriteEntity, IOrderedEntity, IElementEntity<TGroup>
    where TParam : IParam
{
}