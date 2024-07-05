using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IGroupService<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    Task MoveToAnotherParent(Guid groupId, Guid parentId);
    Task CombineChildren(Guid primaryId, Guid secondaryId);
    Task CombineElements(Guid primaryId, Guid secondaryId);
}