using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IGroupService<TGroup, TElement, in TParam> : IGeneralService<TGroup, TParam>
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>, IReferenceEntity
    where TElement : class, IElementReferenceEntity<TGroup, TElement>
    where TParam : class, IParam
{
    Task MoveToAnotherParent(Guid groupId, Guid parentId);
    Task CombineChildren(Guid primaryId, Guid secondaryId);
    Task CombineElements(Guid primaryId, Guid secondaryId);
}