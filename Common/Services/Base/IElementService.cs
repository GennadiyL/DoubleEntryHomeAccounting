using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IElementService<TGroup, TElement, in TParam> : IReferenceService<TElement, TParam>
    where TGroup : class, IGroupEntity<TGroup, TElement>, IReferenceEntity
    where TElement : class, IElementEntity<TGroup, TElement>, IReferenceEntity
    where TParam : class, IParam
{
    Task MoveToAnotherGroup(Guid entityId, Guid groupId);
    Task CombineElements(Guid primaryId, Guid secondaryId);
}