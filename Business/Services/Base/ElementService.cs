using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;

public abstract class ElementService<TGroup, TElement, TParam> : IElementService<TGroup, TElement, TParam>
    where TGroup : class, IGroupEntity<TGroup, TElement>, IReferenceEntity
    where TElement : class, IElementEntity<TGroup, TElement>, IReferenceEntity, new()
    where TParam : class, INamedParam, IFavoriteParam, IElementParam
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    protected ElementService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(TParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TGroup group = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, param.GroupId);
        ICollection<TElement> elements = group.Elements;

        Guard.CheckEntityWithSameName(elements, Guid.Empty, param.Name);

        TElement addedEntity = new TElement
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order =  elements.GetMaxOrder() + 1,
            GroupId = param.GroupId
        };

        elements.Add(addedEntity);

        await elementRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement updatedEntity = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        ICollection<TElement> elements = await elementRepository.GetElementsByGroupId(updatedEntity.GroupId);
        Guard.CheckEntityWithSameName(elements, updatedEntity.Id, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await elementRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TElement deletedEntity = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        ICollection<TElement> elements = await elementRepository.GetElementsByGroupId(deletedEntity.GroupId);

        ICollection<Account> accounts = await GetAccountsByEntity(accountRepository, deletedEntity);
        foreach (Account account in accounts)
        {
            AccountEntitySetter(default, account);
            await accountRepository.Update(account);
        }

        elements.Remove(deletedEntity);
        elements.Reorder();

        await elementRepository.Delete(deletedEntity.Id);
        await elementRepository.Update(elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement entity = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        if (entity.Order == order)
        {
            return;
        }

        TGroup group = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, entity.GroupId);
        group.Elements.SetOrder(entity, order);

        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement entity = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;

        await elementRepository.Update(entity);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement entity = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        TGroup fromGroup = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, entity.GroupId);
        TGroup toGroup = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, groupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(elementRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await elementRepository.GetMaxOrderInGroup(toGroup.Id) + 1;

        fromGroup.Elements.Remove(entity);
        entity.Group = toGroup;
        toGroup.Elements.Add(entity);

        fromGroup.Elements.Reorder();
        toGroup.Elements.SetOrder(entity, newOrder);

        await elementRepository.Update(toGroup.Elements);
        await elementRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TElement primaryEntity = await Guard.CheckAndGetEntityById(elementRepository.GetById, primaryId);
        TElement secondaryEntity = await Guard.CheckAndGetEntityById(elementRepository.GetById, secondaryId);

        if (primaryEntity.Id == secondaryEntity.Id)
        {
            return;
        }

        ICollection<Account> accounts = await GetAccountsByEntity(accountRepository, secondaryEntity);
        foreach (Account account in accounts)
        {
            AccountEntitySetter(primaryEntity, account);
            await accountRepository.Update(account);
        }

        TGroup group = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, secondaryEntity.GroupId);
        group.Elements.Remove(secondaryEntity);
        group.Elements.Reorder();

        await elementRepository.Delete(secondaryEntity.Id);
        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    protected abstract Func<IAccountRepository, TElement, Task<ICollection<Account>>> GetAccountsByEntity { get; }
    
    protected abstract Action<TElement, Account> AccountEntitySetter { get; }
}