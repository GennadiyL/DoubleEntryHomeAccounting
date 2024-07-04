using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceDataGroupService<TGroup, TElement, in TParam> : IReferenceDataService<TElement, TParam>, IGroupService<TGroup, TElement>
    where TGroup : class, IEntity, IFavoriteEntity, IOrderedEntity, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IEntity, IFavoriteEntity, IOrderedEntity, IReferenceDataElementEntity<TGroup, TElement>
    where TParam : class, IParam
{
}