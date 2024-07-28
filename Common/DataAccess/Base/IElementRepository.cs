using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    Task<ICollection<TElement>> GetElementsByName(string name);
    Task<ICollection<TElement>> GetElementsByGroupId(Guid groupId);
    Task<TGroup> GetGroupByGroupId(Guid groupId);
    Task<int> GetMaxOrder(Guid groupId);
    Task<int> GetCount(Guid groupId);
}