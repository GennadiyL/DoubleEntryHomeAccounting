using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;

public static class Guard
{
    public static void CheckParamForNull<T>(T entity)
    {
        if (entity == null) 
        {
            throw new MissingInputParameterException(typeof(T).Name);
        }
    }

    public static void CheckParamNameForNull<T>(T entity) where T : INamedParam
    {
        if (entity.Name == null)
        {
            throw new ArgumentNullException($"In {typeof(T).Name} the Name cannot be a null");
        }
    }

    public static async Task CheckGroupWithSameName<TGroup, TElement>(IGroupRepository<TGroup, TElement> repository, Guid? parentId, Guid id, string name)
        where TGroup : class, IGroupEntity<TGroup, TElement>, INamedEntity
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        TGroup group = await repository.GetByParentId(parentId);
        if (group == null)
        {
            return;
        }
        CheckEntityWithSameName(group.Children, id, name);
    }

    public static async Task CheckElementWithSameName<TGroup, TElement>(IElementRepository<TGroup, TElement> repository, Guid groupId, Guid id, string name)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>, INamedEntity
    {
        TGroup group = await repository.GetByGroupId(groupId);
        CheckEntityWithSameName(group.Elements, id, name);
    }

    private static void CheckEntityWithSameName<T>(ICollection<T> entities, Guid id, string name)
        where T : INamedEntity
    {
        if (entities
                .Where(e => string.Equals(e.Name, name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault(t => t.Id != id) != null)
        {
            throw new ArgumentException($"In {typeof(T).Name} with the same name '{name}' has already existed in the collection");
        }
    }

    public static async Task CheckExistedElementsInTheGroup<TGroup, TElement>(IElementRepository<TGroup, TElement> repository, Guid groupId)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        int count = await repository.GetCount(groupId);
        if (count > 0)
        {
            throw new GroupContainsElementException(typeof(TGroup), count);
        }
    }

    public static async Task CheckExistedChildrenInTheGroup<TGroup, TElement>(IGroupRepository<TGroup, TElement> repository, Guid groupId)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        int count = await repository.GetCount(groupId);
        if (count > 0)
        {
            throw new GroupContainsSubGroupsException(typeof(TGroup), count);
        }
    }
}