using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;

public abstract class ReferenceDataGroupService<TGroup, TElement, TParam> : IReferenceDataGroupService<TGroup, TElement, TParam>
    where TGroup : class, IGroupEntity<TGroup, TElement>, IReferenceDataEntity, new()
    where TElement : class, IElementEntity<TGroup, TElement>
    where TParam : class, INamedParam, IFavoriteParam, IGroupParam

{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    protected ReferenceDataGroupService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TGroup parent = param.ParentId.HasValue ? await Getter.GetEntityById(g => groupRepository.GetById(g), param.ParentId.Value) : default;
        await Guard.CheckGroupWithSameName(groupRepository, parent?.Id, Guid.Empty, param.Name);

        TGroup addedEntity = new TGroup
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await groupRepository.GetMaxOrder(parent?.Id) + 1
        };

        addedEntity.Parent = parent;
        parent?.Children.Add(addedEntity);

        await groupRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TGroup updatedEntity = await Getter.GetEntityById(g => groupRepository.GetById(g), entityId);
        await Guard.CheckGroupWithSameName(groupRepository, updatedEntity.ParentId, updatedEntity.Id, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await groupRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();
        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();

        TGroup deletedEntity = await Getter.GetEntityById(g => groupRepository.GetById(g), entityId);

        await Guard.CheckExistedChildrenInTheGroup(groupRepository, deletedEntity.Id);
        await Guard.CheckExistedElementsInTheGroup(elementRepository, deletedEntity.Id);

        TGroup parent = await groupRepository.GetByParentId(deletedEntity.ParentId);

        ICollection<TGroup> entities;
        if (parent != null)
        {
            parent.Children.Remove(deletedEntity);
            entities = parent.Children;
        }
        else
        {
            entities = await groupRepository.Where(e => e.Parent == null);
        }
        OrderingUtils.Reorder(entities);

        await groupRepository.Delete(deletedEntity.Id);
        await groupRepository.Update(entities);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();

        TGroup entity = await Getter.GetEntityById(g => groupRepository.GetById(g), entityId);
        if (entity.Order == order)
        {
            return;
        }

        ICollection<TGroup> entities;
        if (entity.ParentId.HasValue)
        {
            TGroup parent = await groupRepository.GetByParentId(entity.ParentId);
            entities = parent.Children;
        }
        else
        {
            entities = await groupRepository.Where(e => e.Parent == null);
        }

        OrderingUtils.SetOrder(entities, entity, order);

        await groupRepository.Update(entities);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();

        TGroup entity = await Getter.GetEntityById(g => groupRepository.GetById(g), entityId);
        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;

        await groupRepository.Update(entity);

        await unitOfWork.SaveChanges();
    }

    //TODO: Implementation
    public Task MoveToAnotherParent(Guid groupId, Guid parentId)
    {
        throw new NotImplementedException();
    }

    //TODO: Implementation
    public Task CombineChildren(Guid primaryId, Guid secondaryId)
    {
        throw new NotImplementedException();
    }

    //TODO: Implementation
    public Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        throw new NotImplementedException();
    }
}