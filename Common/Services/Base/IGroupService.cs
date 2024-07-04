using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

public interface IGroupService<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
    Task MoveToAnotherParent(Guid groupId, Guid parentId);
    Task CombineGroups(Guid primaryId, Guid secondaryId);
    Task CombineElements(Guid primaryId, Guid secondaryId);
}