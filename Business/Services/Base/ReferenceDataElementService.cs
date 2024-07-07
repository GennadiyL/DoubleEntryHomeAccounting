using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services.Base;

public abstract class ReferenceDataElementService<TGroup, TElement, TParam> : IReferenceDataElementService<TGroup, TElement, TParam>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>, IReferenceDataEntity, new()
    where TParam : class, INamedParam, IFavoriteParam, IElementParam
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ReferenceDataElementService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IGroupEntityRepository<TGroup, TElement> groupRepository = unitOfWork.GetRepository<IGroupEntityRepository<TGroup, TElement>>();
        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TGroup group = await Getter.GetEntityById(g => groupRepository.GetById(g), param.GroupId);
        await Guard.CheckElementWithSameName(elementRepository, group.Id, Guid.Empty, param.Name);

        TElement addedEntity = new TElement
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await elementRepository.GetMaxOrder(group.Id) + 1
        };

        addedEntity.Group = group;
        group.Elements.Add(addedEntity);

        await elementRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TElement updatedEntity = await Getter.GetEntityById(g => elementRepository.GetById(g), entityId);
        await Guard.CheckElementWithSameName(elementRepository, updatedEntity.GroupId, entityId, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await elementRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TElement deletedEntity = await Getter.GetEntityById(g => elementRepository.GetById(g), entityId);

        TGroup group = await elementRepository.GetByGroupId(deletedEntity.GroupId);

        ICollection<Account> accounts = await GetAccountsByEntity(accountRepository, deletedEntity);
        foreach (Account account in accounts)
        {
            AccountEntitySetter(default, account);
            await accountRepository.Update(account);
        }

        group.Elements.Remove(deletedEntity);
        OrderingUtils.Reorder(group.Elements);

        await elementRepository.Delete(deletedEntity.Id);
        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();

        TElement entity = await Getter.GetEntityById(g => elementRepository.GetById(g), entityId);
        if (entity.Order == order)
        {
            return;
        }

        TGroup group = await elementRepository.GetByGroupId(entity.GroupId);
        OrderingUtils.SetOrder(group.Elements, entity, order);

        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();

        TElement entity = await Getter.GetEntityById(g => elementRepository.GetById(g), entityId);
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
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();

        TElement entity = await Getter.GetEntityById(g => elementRepository.GetById(g), entityId);
        TGroup fromGroup = await elementRepository.GetByGroupId(entity.GroupId);
        TGroup toGroup = await elementRepository.GetByGroupId(groupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(elementRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await elementRepository.GetMaxOrder(toGroup.Id) + 1;

        fromGroup.Elements.Remove(entity);
        entity.Group = toGroup;
        toGroup.Elements.Add(entity);

        OrderingUtils.Reorder(fromGroup.Elements);
        OrderingUtils.SetOrder(toGroup.Elements, entity, newOrder);

        await elementRepository.Update(toGroup.Elements);
        await elementRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IElementEntityRepository<TGroup, TElement> elementRepository = unitOfWork.GetRepository<IElementEntityRepository<TGroup, TElement>>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        TElement primaryEntity = await Getter.GetEntityById(g => elementRepository.GetById(g), primaryId);
        TElement secondaryEntity = await Getter.GetEntityById(g => elementRepository.GetById(g), secondaryId);

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

        TGroup group = await elementRepository.GetByGroupId(secondaryEntity.GroupId);
        group.Elements.Remove(secondaryEntity);
        OrderingUtils.Reorder(group.Elements);

        await elementRepository.Delete(secondaryEntity.Id);
        await elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    protected abstract Func<IAccountRepository, TElement, Task<ICollection<Account>>> GetAccountsByEntity { get; }
    protected abstract Action<TElement, Account> AccountEntitySetter { get; }
}