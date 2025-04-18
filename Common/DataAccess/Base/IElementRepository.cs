using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<TGroup, TElement> : IEntityRepository<TElement>
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement : class, IElementReferenceEntity<TGroup, TElement>
{
    Task<ICollection<TElement>> GetByName(string name);
    [Obsolete("Use GetGroupWithElementsByGroupId instead")]
    Task<ICollection<TElement>> GetElementsByGroupId(Guid groupId);
    //TODO: REVIEW
    //Rename to GetElementsByGroupId
    Task<TGroup> GetGroupWithElementsByGroupId(Guid groupId);
    Task<int> GetMaxOrderInGroup(Guid groupId);
    Task<int> GetCountInGroup(Guid groupId);
}