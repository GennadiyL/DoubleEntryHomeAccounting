using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IGroupRepository<TGroup, TElement> : IEntityRepository<TGroup>
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement : class, IElementReferenceEntity<TGroup, TElement>
{
    Task<ICollection<TGroup>> GetByName(string name);
    [Obsolete("Use GetParentWithChildrenByParentId")]
    Task<ICollection<TGroup>> GetChildrenByParentId(Guid? parentId);
    Task<TGroup> GetParentWithChildrenByParentId(Guid parentId);
    Task<int> GetMaxOrderInGroup(Guid? parentId);
    Task<int> GetCountInGroup(Guid? parentId);
}