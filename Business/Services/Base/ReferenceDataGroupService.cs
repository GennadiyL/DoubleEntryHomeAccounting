using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;
using System.Xml.Linq;

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

        TGroup parent = null;
        ICollection<TGroup> children;
        if (param.ParentId.HasValue)
        {
            parent = await Guard.CheckAndGetEntityById(groupRepository.GetParentByParentId, param.ParentId.Value);
            children = parent.Children;
        }
        else
        {
            children = await groupRepository.GetChildrenByParentId(default);
            
        }
        Guard.CheckEntityWithSameName(children, Guid.Empty, param.Name);

        TGroup addedEntity = new TGroup
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = children.GetMaxOrder() + 1,
            Parent = parent
        };

        children.Add(addedEntity);

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

        TGroup updatedEntity = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        ICollection<TGroup> children = await groupRepository.GetChildrenByParentId(updatedEntity.ParentId);
        Guard.CheckEntityWithSameName(children, updatedEntity.Id, param.Name);

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

        TGroup deletedEntity = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        ICollection<TGroup> children = await groupRepository.GetChildrenByParentId(deletedEntity.ParentId);

        await Guard.CheckExistedChildrenInTheGroup(groupRepository, deletedEntity.Id);
        await Guard.CheckExistedElementsInTheGroup(elementRepository, deletedEntity.Id);

        children.Remove(deletedEntity);
        children.Reorder();

        await groupRepository.Delete(deletedEntity.Id);
        await groupRepository.Update(children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();

        TGroup entity = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        if (entity.Order == order)
        {
            return;
        }

        ICollection<TGroup> entities;
        if (entity.ParentId.HasValue)
        {
            TGroup parent = await groupRepository.GetParentByParentId(entity.ParentId);
            entities = parent.Children;
        }
        else
        {
            entities = await groupRepository.Where(e => e.Parent == null);
        }

        entities.SetOrder(entity, order);

        await groupRepository.Update(entities);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();

        TGroup entity = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
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