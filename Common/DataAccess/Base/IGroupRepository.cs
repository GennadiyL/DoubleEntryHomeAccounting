using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IGroupRepository<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    Task<ICollection<TGroup>> GetByName(string name);
    Task<TGroup> GetByParentId(Guid? parentId);
    Task<TGroup> GetByParentIdAndName(Guid? parentId, string name);
    Task<int> GetMaxOrder(Guid? parentId);
    Task<int> GetCount(Guid? parentId);
}