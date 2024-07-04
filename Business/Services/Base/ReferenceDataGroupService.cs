using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace Business.Services.Base;

public abstract class ReferenceDataGroupService<TGroup, TElement, TParam> : IReferenceDataGroupService<TGroup, TElement, TParam>
    where TGroup : IReferenceDataGroupEntity<TGroup, TElement>, new()
    where TElement : IReferenceDataElementEntity<TGroup>
    where TParam : INamedParam, IFavoriteParam, IGroupParam

{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IGroupEntityRepository<TGroup, TElement> _groupRepository;
    private readonly IElementEntityRepository<TGroup, TElement> _elementRepository;

    protected IUnitOfWorkFactory UnitOfWorkFactory => _unitOfWorkFactory;

    protected ReferenceDataGroupService(
        IUnitOfWorkFactory unitOfWorkFactory, 
        IGroupEntityRepository<TGroup, TElement> groupRepository, 
        IElementEntityRepository<TGroup, TElement> elementRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _groupRepository = groupRepository;
        _elementRepository = elementRepository;
    }

    public async Task<Guid> Add(TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TGroup parent = param.ParentId.HasValue ? await Getter.GetEntityById(_groupRepository, param.ParentId.Value) : default;
        await Guard.CheckGroupWithSameName(_groupRepository, parent?.Id, Guid.Empty, param.Name);

        TGroup addedEntity = new TGroup
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await _groupRepository.GetMaxOrder(parent?.Id) + 1
        };

        addedEntity.Parent = parent;
        parent?.Children.Add(addedEntity);

        await _groupRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TGroup updatedEntity = await Getter.GetEntityById(_groupRepository, entityId);

        TGroup parent = updatedEntity.Parent != null ? await _groupRepository.GetById(updatedEntity.Parent.Id) : default;
        await Guard.CheckGroupWithSameName(_groupRepository, parent?.Id, updatedEntity.Id, updatedEntity.Name);
        
        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await _groupRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TGroup deletedEntity = await Getter.GetEntityById(_groupRepository, entityId);

        await Guard.CheckExistedSubgroupsInTheGroup(_groupRepository, deletedEntity.Id);
        await Guard.CheckExistedElementsInTheGroup(_elementRepository, deletedEntity.Id);

        TGroup parent = deletedEntity.Parent != null ? await _groupRepository.GetByParentId(deletedEntity.Parent.Id) : default;
        
        await _groupRepository.Delete(deletedEntity.Id);

        ICollection<TGroup> entities;
        if (parent != null)
        {
            parent.Children.Remove(deletedEntity);
            entities = parent.Children;
        }
        else
        {
            entities = await _groupRepository.Where(e => e.Parent == null);
        }
        OrderingUtils.Reorder(entities);
        await _groupRepository.Update(entities);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TGroup entity = await Getter.GetEntityById(_groupRepository, entityId);
        if (entity.Order != order)
        {
            ICollection<TGroup> entities;
            if (entity.Parent != null)
            {
                TGroup parent = await _groupRepository.GetByParentId(entity.Parent.Id);
                entities = parent.Children;
            }
            else
            {
                entities = await _groupRepository.Where(e => e.Parent == null);
            }
            OrderingUtils.SetOrder(entities, entity, order);
            await _groupRepository.Update(entities);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TGroup entity = await Getter.GetEntityById(_groupRepository, entityId);
        if (entity.IsFavorite != isFavorite)
        {
            entity.IsFavorite = isFavorite;
            await _groupRepository.Update(entity);
        }

        await unitOfWork.SaveChanges();
    }


    public Task MoveToAnotherParent(Guid groupId, Guid parentId)
    {
        throw new NotImplementedException();
    }

    public Task CombineGroups(Guid primaryId, Guid secondaryId)
    {
        throw new NotImplementedException();
    }

    public Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        throw new NotImplementedException();
    }

}