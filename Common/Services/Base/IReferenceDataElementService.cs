using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IReferenceDataElementService<TGroup, TElement, in TParam> : IReferenceDataService<TElement, TParam>, IElementService<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
    where TParam : class, IParam
{
}