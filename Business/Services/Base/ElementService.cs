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
        Guard.CheckParamNameForNullOrEmpty(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TGroup group = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, param.GroupId);

        Guard.CheckEntityWithSameName(group.Elements, Guid.Empty, param.Name);

        TElement addedEntity = new TElement
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = group.Elements.GetMaxOrder() + 1,
            Group = group,
            GroupId = param.GroupId
        };

        group.Elements.Add(addedEntity);

        await elementRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNullOrEmpty(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement updatedElement = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);

        TGroup group = await elementRepository.GetGroupWithElementsByGroupId(updatedElement.GroupId);

        Guard.CheckEntityWithSameName(group.Elements, updatedElement.Id, param.Name);

        updatedElement.Name = param.Name;
        updatedElement.Description = param.Description;
        updatedElement.IsFavorite = param.IsFavorite;

        await elementRepository.Update(updatedElement);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TElement deletedElement = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);

        TGroup group = await elementRepository.GetGroupWithElementsByGroupId(deletedElement.GroupId);

        ICollection<Account> accounts = await GetAccountsByEntity(accountRepository, deletedElement);
        foreach (Account account in accounts)
        {
            SetAccountEntity(default, account);
            await accountRepository.Update(account);
        }

        deletedElement.Group = default;
        deletedElement.GroupId = default;
        group.Elements.Remove(deletedElement);
        group.Elements.Reorder();

        await elementRepository.Delete(deletedElement.Id);
        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement element = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        if (element.Order == order)
        {
            return;
        }

        TGroup group = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, element.GroupId);

        group.Elements.SetOrder(element, order);

        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement element = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        if (element.IsFavorite == isFavorite)
        {
            return;
        }

        element.IsFavorite = isFavorite;

        await elementRepository.Update(element);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid toGroupId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();

        TElement element = await Guard.CheckAndGetEntityById(elementRepository.GetById, entityId);
        TGroup fromGroup = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, element.GroupId);
        TGroup toGroup = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, toGroupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(elementRepository, toGroup.Id, element.Id, element.Name);

        element.Group = toGroup;
        element.GroupId = toGroup.Id;
        element.Order = await elementRepository.GetMaxOrderInGroup(toGroup.Id) + 1;
        toGroup.Elements.Add(element);

        fromGroup.Elements.Remove(element);
        fromGroup.Elements.Reorder();

        await elementRepository.Update(toGroup.Elements);
        await elementRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid toElementId, Guid fromElementId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementRepository<TGroup, TElement>>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TElement toElement = await Guard.CheckAndGetEntityById(elementRepository.GetById, toElementId);
        TElement fromElement = await Guard.CheckAndGetEntityById(elementRepository.GetById, fromElementId);

        if (toElement.Id == fromElement.Id)
        {
            return;
        }

        ICollection<Account> accounts = await GetAccountsByEntity(accountRepository, fromElement);
        foreach (Account account in accounts)
        {
            SetAccountEntity(toElement, account);
            await accountRepository.Update(account);
        }

        TGroup group = await Guard.CheckAndGetEntityById(elementRepository.GetGroupWithElementsByGroupId, fromElement.GroupId);
        fromElement.Group = default;
        fromElement.GroupId = default;
        group.Elements.Remove(fromElement);
        group.Elements.Reorder();

        await elementRepository.Delete(fromElement.Id);
        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    protected abstract Func<IAccountRepository, TElement, Task<ICollection<Account>>> GetAccountsByEntity { get; }

    protected abstract Action<TElement, Account> SetAccountEntity { get; }
}