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
            Parent = parent,
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
        Guard.CheckIsRoot<TGroup, TElement>(updatedGroup);

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
        Guard.CheckIsRoot<TGroup, TElement>(deletedGroup);

        TGroup parent = await groupRepository.GetParentWithChildrenByParentId(deletedGroup.ParentId);

        await Guard.CheckExistedChildrenInTheParent(groupRepository, deletedGroup.Id);
        await Guard.CheckExistedElementsInTheGroup(elementRepository, deletedGroup.Id);

        deletedGroup.Parent = default;
        deletedGroup.ParentId = default;
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

        TGroup group = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        if (group.Order == order)
        {
            return;
        }

        TGroup parent = await groupRepository.GetParentWithChildrenByParentId(group.ParentId);

        parent.Children.SetOrder(group, order);

        await groupRepository.Update(parent.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();

        TGroup group = await Guard.CheckAndGetEntityById(groupRepository.GetById, entityId);
        if (group.IsFavorite == isFavorite)
        {
            return;
        }

        group.IsFavorite = isFavorite;

        await groupRepository.Update(group);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherParent(Guid groupId, Guid toParentId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupRepository<TGroup, TElement>>();
        
        TGroup group = await Guard.CheckAndGetEntityById(groupRepository.GetById, groupId);
        TGroup fromParent = await Guard.CheckAndGetEntityById(groupRepository.GetParentWithChildrenByParentId, group.ParentId);
        TGroup toParent = await Guard.CheckAndGetEntityById(groupRepository.GetParentWithChildrenByParentId, toParentId);

        if (fromParent.Id == toParent.Id)
        {
            return;
        }

        await Guard.CheckGroupWithSameName(groupRepository, toParent.Id, group.Id, group.Name);
        await Guard.CheckCycle(groupRepository, toParent.Id, group.Id);

        group.Parent = toParent;
        group.ParentId = toParent.Id;
        group.Order = await groupRepository.GetMaxOrderInParent(toParent.Id) + 1;
        toParent.Children.Add(group);

        fromParent.Children.Remove(group);
        fromParent.Children.Reorder();

        await groupRepository.Update(toParent.Children);
        await groupRepository.Update(fromParent.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineGroups(Guid toGroupId, Guid fromGroupId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TGroup toGroup = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, toGroupId);
        TGroup fromGroup = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, fromGroupId);

        if (toGroup.Id == fromGroup.Id)
        {
            return;
        }

        int nextElementOrder = await elementRepository.GetMaxOrderInGroup(toGroupId) + 1;
        foreach (TElement element in fromGroup.Elements)
        {
            fromGroup.Elements.Remove(element);
            element.Group = toGroup;
            element.GroupId = toGroup.Id;
            element.Order = nextElementOrder++;
            toGroup.Elements.Add(element);
        }

        await elementRepository.Update(fromGroup.Elements);
        await elementRepository.Update(toGroup.Elements);

        await unitOfWork.SaveChanges();
    }
}