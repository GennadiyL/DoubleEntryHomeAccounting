using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;

public static class Guard
{
    public static void CheckParamForNull<T>(T param) where T : class
    {
        if (param == null)
        {
            throw new NullParameterException(typeof(T));
        }
    }

    public static void CheckParamNameForNullOrEmpty<T>(T param) where T : INamedParam
    {
        if (param.Name == null)
        {
            throw new NullNameException(typeof(T));
        }
    }

    public static void CheckEnumeration<T>(T param) where T : struct
    {
        if (!((T[])Enum.GetValues(typeof(T))).Contains(param))
        {
            throw new InvalidEnumerationException(typeof(T), param);
        }
    }

    public static async Task CheckGroupWithSameName<TGroup, TElement>(
        IGroupRepository<TGroup, TElement> repository,
        Guid parentId,
        Guid id,
        string name)
        where TGroup : class, IEntity, INamedEntity, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        TGroup parent = await repository.GetParentWithChildrenByParentId(parentId);
        CheckEntityWithSameName(parent.Children, id, name);
    }

    public static async Task CheckElementWithSameName<TGroup, TElement>(
        IElementRepository<TGroup, TElement> repository,
        Guid groupId,
        Guid id,
        string name)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IEntity, INamedEntity, IElementEntity<TGroup, TElement>
    {
        TGroup group = await repository.GetGroupWithElementsByGroupId(groupId);
        CheckEntityWithSameName(group.Elements, id, name);
    }

    public static void CheckEntityWithSameName<T>(ICollection<T> entities, Guid id, string name)
        where T : IEntity, INamedEntity
    {
        if (entities.Where(e => string.Equals(e.Name, name,
                StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(t => t.Id != id) != null)
        {
            throw new DuplicationNameException(typeof(T), name);
        }
    }

    public static async Task CheckExistedChildrenInTheParent<TGroup, TElement>(
        IGroupRepository<TGroup, TElement> groupRepository,
        Guid? parentId)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        int count = await groupRepository.GetCountInParent(parentId);
        if (count > 0)
        {
            throw new GroupContainsSubGroupsException(typeof(TGroup), count);
        }
    }

    public static async Task CheckExistedElementsInTheGroup<TGroup, TElement>(
        IElementRepository<TGroup, TElement> elementRepository,
        Guid groupId)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        int count = await elementRepository.GetCountInGroup(groupId);
        if (count > 0)
        {
            throw new GroupContainsElementException(typeof(TGroup), count);
        }
    }

    public static async Task CheckCycle<TGroup, TElement>(
        IGroupRepository<TGroup, TElement> groupRepository,
        Guid parentId,
        Guid childId)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        TGroup checkGroup = await CheckAndGetEntityById(groupRepository.GetById, parentId);
        while (checkGroup.Id != childId)
        {
            if (Roots.IsRoot<TGroup, TElement>(checkGroup))
            {
                return;
            }

            checkGroup = await groupRepository.GetById(checkGroup.ParentId);
        }

        throw new GroupCycleException(typeof(TGroup));
    }


    public static void CheckIsRoot<TGroup, TElement>(TGroup group)
        where TGroup : class, IGroupEntity<TGroup, TElement>
        where TElement : class, IElementEntity<TGroup, TElement>
    {
        if (Roots.IsRoot<TGroup, TElement>(group))
        {
            throw new ReadonlyRootGroupException(typeof(TGroup));
        }
    }

    public static async Task<T> CheckAndGetEntityById<T>(Func<Guid, Task<T>> func, Guid entityId) where T : IEntity
    {
        T item = await func(entityId) ?? throw new MissingEntityException(typeof(T), entityId);
        return item;
    }
}