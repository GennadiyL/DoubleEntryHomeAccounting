using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;

public abstract class GroupService<TGroup, TElement, TParam> : IGroupService<TGroup, TElement, TParam>
    where TGroup : class, IGroupEntity<TGroup, TElement>, IReferenceEntity, new()
    where TElement : class, IElementEntity<TGroup, TElement>, IReferenceEntity
    where TParam : class, INamedParam, IFavoriteParam, IGroupParam
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    protected GroupService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(TParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNullOrEmpty(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup parent = await Guard.CheckAndGetEntityById(groupRepository.GetParentWithChildrenByParentId, param.ParentId);

        Guard.CheckEntityWithSameName(parent.Children, Guid.Empty, param.Name);

        TGroup addedGroup = new TGroup
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = parent.Children.GetMaxOrder() + 1,
            ParentId = param.ParentId
        };

        parent.Children.Add(addedGroup);

        await groupRepository.Add(addedGroup);

        await unitOfWork.SaveChanges();

        return addedGroup.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNullOrEmpty(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup updatedGroup = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);

        TGroup parent = await groupRepository.GetParentWithChildrenByParentId(updatedGroup.ParentId);

        Guard.CheckEntityWithSameName(parent.Children, updatedGroup.Id, param.Name);

        updatedGroup.Name = param.Name;
        updatedGroup.Description = param.Description;
        updatedGroup.IsFavorite = param.IsFavorite;

        await groupRepository.Update(updatedGroup);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();
        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TGroup deletedGroup = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);

        TGroup parent = await groupRepository.GetParentWithChildrenByParentId(deletedGroup.ParentId);

        await Guard.CheckExistedChildrenInTheParent(groupRepository, deletedGroup.Id);
        await Guard.CheckExistedElementsInTheGroup(elementRepository, deletedGroup.Id);

        parent.Children.Remove(deletedGroup);
        parent.Children.Reorder();

        await groupRepository.Delete(deletedGroup.Id);
        await groupRepository.Update(parent.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup entity = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        if (entity.Order == order)
        {
            return;
        }

        TGroup parent = await groupRepository.GetParentWithChildrenByParentId(entity.ParentId);

        parent.Children.SetOrder(entity, order);

        await groupRepository.Update(parent.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup entity = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;

        await groupRepository.Update(entity);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherParent(Guid groupId, Guid toParentId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup entity = await Guard.CheckAndGetEntityById(groupRepository.GetById, groupId);
        TGroup fromParent = await Guard.CheckAndGetEntityById(groupRepository.GetParentWithChildrenByParentId, entity.ParentId);
        TGroup toParent = await Guard.CheckAndGetEntityById(groupRepository.GetParentWithChildrenByParentId, toParentId);

        if (fromParent.Id == toParent.Id)
        {
            return;
        }

        await Guard.CheckGroupWithSameName(groupRepository, toParent.Id, entity.Id, entity.Name);

        entity.Parent = toParent;
        entity.ParentId = toParent.Id;
        entity.Order = await groupRepository.GetMaxOrderInParent(toParent.Id) + 1;
        toParent.Children.Add(entity);

        fromParent.Children.Remove(entity);
        fromParent.Children.Reorder();

        await groupRepository.Update(toParent.Children);
        await groupRepository.Update(fromParent.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineChildren(Guid toGroupId, Guid fromGroupId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();
        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup toGroup = await Guard.CheckAndGetEntityById(groupRepository.GetById, toGroupId);
        TGroup fromGroup = await Guard.CheckAndGetEntityById(groupRepository.GetById, fromGroupId);

        if (toGroup.Id == fromGroup.Id)
        {
            return;
        }

        toGroup = await elementRepository.GetGroupWithElementsByGroupId(toGroupId);
        fromGroup = await elementRepository.GetGroupWithElementsByGroupId(fromGroupId);
        int nextElementOrder = await elementRepository.GetMaxOrderInGroup(toGroupId) + 1;
        foreach (TElement element in fromGroup.Elements)
        {
            fromGroup.Elements.Remove(element);
            element.Order = nextElementOrder++;
            toGroup .Elements.Add(element);
        }

        toGroup = await groupRepository.GetParentWithChildrenByParentId(toGroupId);
        fromGroup = await groupRepository.GetParentWithChildrenByParentId(fromGroupId);
        int nextChildOrder = await groupRepository.GetMaxOrderInParent(toGroupId) + 1;
        foreach (TGroup child in fromGroup .Children)
        {
            fromGroup.Children.Remove(child);
            child.Order = nextChildOrder++;
            toGroup .Children.Add(child);
        }
        
        TGroup parent = await Guard.CheckAndGetEntityById(groupRepository.GetParentWithChildrenByParentId, fromGroup.ParentId);
        parent.Children.Remove(fromGroup);
        parent.Children.Reorder();

        await groupRepository.Delete(fromGroup.Id);
        await groupRepository.Update(parent.Children);

        await unitOfWork.SaveChanges();
    }
}