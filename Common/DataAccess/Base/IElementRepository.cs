using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementRepository<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    Task<ICollection<TElement>> GetByName(string name);
    Task<TGroup> GetByGroupId(Guid groupId);
    [Obsolete]
    Task<TGroup> GetByGroupIdAndName(Guid groupId, string name);
    Task<int> GetMaxOrder(Guid groupId);
    Task<int> GetCount(Guid groupId);
}