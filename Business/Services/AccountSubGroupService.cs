using Common.DataAccess;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;
using Common.Utils.Check;
using Common.Utils.Ordering;

namespace Business.Services;

public class AccountSubGroupService : IAccountSubGroupService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public AccountSubGroupService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Add(AccountSubGroupParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        IAccountGroupRepository accountGroupRepository = await unitOfWork.GetRepository<IAccountGroupRepository>();

        AccountGroup parent = await Getter.GetEntityById(accountGroupRepository.Get, param.ParentId);
        await Guard.CheckEntityWithSameName(accountSubGroupRepository, parent.Id, Guid.Empty, param.Name);

        AccountSubGroup addedEntity = new AccountSubGroup
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await accountSubGroupRepository.GetMaxOrder(parent.Id) + 1
        };

        parent.Children.Add(addedEntity);
        addedEntity.Parent = parent;

        await accountSubGroupRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, AccountSubGroupParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();

        AccountSubGroup updatedEntity = await Getter.GetEntityById(accountSubGroupRepository.Get, entityId);
        await accountSubGroupRepository.LoadParent(updatedEntity);
        AccountGroup parent = updatedEntity.Parent;

        await Guard.CheckEntityWithSameName(accountSubGroupRepository, parent.Id, entityId, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await accountSubGroupRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        IAccountGroupRepository accountGroupRepository =  await unitOfWork.GetRepository<IAccountGroupRepository>();

        AccountSubGroup subGroup = await Getter.GetEntityById(accountSubGroupRepository.Get, entityId);
        await Guard.CheckExistedChildrenInTheGroup(accountRepository, subGroup.Id);

        await accountSubGroupRepository.LoadParent(subGroup);
        AccountGroup group = subGroup.Parent;
        await accountGroupRepository.LoadChildren(group);

        group.Children.Remove(subGroup);
        await accountSubGroupRepository.Delete(subGroup);

        OrderingUtils.Reorder(group.Children);
        await accountSubGroupRepository.UpdateList(group.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();

        AccountSubGroup entity = await Getter.GetEntityById(accountSubGroupRepository.Get, entityId);
        if (entity.IsFavorite != isFavorite)
        {
            entity.IsFavorite = isFavorite;
            await accountSubGroupRepository.Update(entity);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        IAccountGroupRepository accountGroupRepository = await unitOfWork.GetRepository<IAccountGroupRepository>();

        AccountSubGroup entity = await Getter.GetEntityById(accountSubGroupRepository.Get, entityId);
        if (entity.Order != order)
        {
            await accountSubGroupRepository.LoadParent(entity);
            AccountGroup group = entity.Parent;
            await accountGroupRepository.LoadChildren(group);
            OrderingUtils.SetOrder(group.Children, entity, order);
            await accountSubGroupRepository.UpdateList(group.Children);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherParent(Guid entityId, Guid parentId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        IAccountGroupRepository accountGroupRepository = await unitOfWork.GetRepository<IAccountGroupRepository>();

        AccountSubGroup entity = await Getter.GetEntityById(accountSubGroupRepository.Get, entityId);
        await accountSubGroupRepository.LoadParent(entity);
        AccountGroup fromParent = entity.Parent;

        AccountGroup toParent = await Getter.GetEntityById(accountGroupRepository.Get, parentId);

        if (fromParent.Id != toParent.Id)
        {
            await Guard.CheckEntityWithSameName(accountSubGroupRepository, toParent.Id, entity.Id, entity.Name);

            await accountGroupRepository.LoadChildren(fromParent);

            entity.Order = await accountSubGroupRepository.GetMaxOrder(toParent.Id) + 1;

            fromParent.Children.Remove(entity);
            toParent.Children.Add(entity);
            entity.Parent = toParent;
            await accountSubGroupRepository.Update(entity);

            OrderingUtils.Reorder(fromParent.Children);
            await accountSubGroupRepository.UpdateList(fromParent.Children);
        }

        await unitOfWork.SaveChanges();
    }
}