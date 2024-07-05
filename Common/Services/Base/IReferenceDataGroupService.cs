using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceDataGroupService<TGroup, TElement, in TParam> : IReferenceDataService<TGroup, TParam>, IGroupService<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>, IReferenceDataEntity
    where TElement : class, IElementEntity<TGroup, TElement>
    where TParam : class, IParam
{
}