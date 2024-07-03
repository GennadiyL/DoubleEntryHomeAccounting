﻿using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;

public static class Guard
{
    public static void CheckParamForNull<T>(T entity)
    {
        if (entity == null) 
        {
            throw new ArgumentNullException($"{typeof(T).Name} cannot be a null");
        }
    }

    public static void CheckParamNameForNull<T>(T entity) where T : INamedParam
    {
        if (entity.Name == null)
        {
            throw new ArgumentNullException($"In {typeof(T).Name} the Name cannot be a null");
        }
    }

    public static async Task CheckGroupWithSameName<TGroup, TElement>(IGroupRepository<TGroup, TElement> repository, Guid parentId, Guid id, string name)
        where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
        where TElement : IReferenceDataElementEntity<TGroup>
    {
        ICollection<TGroup> entities = await repository.GetByParentIdAndName(parentId, name);
        CheckEntityWithSameName(entities, id, name);
    }

    public static async Task CheckElementWithSameName<TGroup, TElement>(IElementRepository<TGroup, TElement> repository, Guid groupId, Guid id, string name)
        where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
        where TElement : IReferenceDataElementEntity<TGroup>
    {
        ICollection<TElement> entities = await repository.GetByName(groupId, name);
        CheckEntityWithSameName(entities, id, name);
    }

    private static void CheckEntityWithSameName<T>(ICollection<T> entities, Guid id, string name)
        where T : IEntity, INamedEntity
    {
        if (entities
                .Where(e => string.Equals(e.Name, name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault(t => t.Id != id) != null)
        {
            throw new ArgumentException($"In {typeof(T).Name} with the same name has already existed in the collection");
        }
    }

    public static async Task CheckExistedElementsInTheGroup<TGroup, TElement>(IElementRepository<TGroup, TElement> repository, Guid parentId)
        where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
        where TElement : IReferenceDataElementEntity<TGroup>
    {
        if (await repository.GetCount(parentId) > 0)
        {
            throw new ArgumentException("Group cannot be deleted. It contains elements");
        }
    }

    public static async Task CheckExistedSubgroupsInTheGroup<TGroup, TElement>(IGroupRepository<TGroup, TElement> repository, Guid parentId)
        where TGroup : IReferenceDataGroupEntity<TGroup, TElement>
        where TElement : IReferenceDataElementEntity<TGroup>
    {
        if (await repository.GetCount(parentId) > 0)
        {
            throw new ArgumentException("Group  cannot be deleted. It contains subgroups");
        }
    }

}