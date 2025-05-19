using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IGroupService<TGroup, TElement, in TParam> : IReferenceService<TGroup, TParam>
    where TGroup : class, IGroupEntity<TGroup, TElement>, IReferenceEntity
    where TElement : class, IElementEntity<TGroup, TElement>, IReferenceEntity
    where TParam : class, IParam
{
    Task MoveToAnotherParent(Guid groupId, Guid toParentId);
    Task CombineGroups(Guid toGroupId, Guid fromGroupId);
}