using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;

public interface IGroupRepository<TGroup, TElement> : IEntityRepository<TGroup>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    Task<ICollection<TGroup>> GetByName(string name);
    Task<TGroup> GetParentWithChildrenByParentId(Guid parentId);
    Task<int> GetMaxOrderInParent(Guid? parentId);
    Task<int> GetCountInParent(Guid? parentId);
}