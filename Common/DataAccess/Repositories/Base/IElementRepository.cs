using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;

public interface IElementRepository<TGroup, TElement> : IEntityRepository<TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    Task<ICollection<TElement>> GetByName(string name);
    Task<TGroup> GetGroupWithElementsByGroupId(Guid groupId);
    Task<int> GetMaxOrderInGroup(Guid groupId);
    Task<int> GetCountInGroup(Guid groupId);
}