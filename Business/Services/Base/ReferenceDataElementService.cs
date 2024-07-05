using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace Business.Services.Base;

public abstract class ReferenceDataElementService<TGroup, TElement, TParam> : IReferenceDataElementService<TGroup, TElement, TParam>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>, new()
    where TParam : class, INamedParam, IFavoriteParam, IElementParam
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IGroupEntityRepository<TGroup, TElement> _groupRepository;
    private readonly IElementEntityRepository<TGroup, TElement> _elementRepository;
    private readonly IAccountRepository _accountRepository;

    public ReferenceDataElementService(
        IUnitOfWorkFactory unitOfWorkFactory, 
        IGroupEntityRepository<TGroup, TElement> groupRepository, 
        IElementEntityRepository<TGroup, TElement> elementRepository, 
        IAccountRepository accountRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _groupRepository = groupRepository;
        _elementRepository = elementRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Guid> Add(TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TGroup group = await Getter.GetEntityById(_groupRepository, param.GroupId);
        await Guard.CheckElementWithSameName(_elementRepository, group.Id, Guid.Empty, param.Name);

        TElement addedEntity = new TElement
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await _elementRepository.GetMaxOrder(group.Id) + 1
        };

        addedEntity.Group = group;
        group.Elements.Add(addedEntity);

        await _elementRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        TElement updatedEntity = await Getter.GetElementWithGroupById(_elementRepository, entityId);

        TGroup group = await _elementRepository.GetByGroupId(updatedEntity.Group.Id);
        await Guard.CheckElementWithSameName(_elementRepository, group.Id, entityId, param.Name);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        await _elementRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TElement deletedEntity = await Getter.GetElementWithGroupById(_elementRepository, entityId);

        TGroup group = await _elementRepository.GetByGroupId(deletedEntity.Group.Id);

        ICollection<Account> accounts = await this.GetAccountsByEntity(_accountRepository, deletedEntity);
        foreach (Account account in accounts)
        {
            AccountEntitySetter(default, account);
            await _accountRepository.Update(account);
        }

        group.Elements.Remove(deletedEntity);
        await _elementRepository.Delete(deletedEntity.Id);

        OrderingUtils.Reorder(group.Elements);
        await _elementRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TElement entity = await Getter.GetElementWithGroupById(_elementRepository, entityId);
        if (entity.Order != order)
        {
            TGroup group = await _elementRepository.GetByGroupId(entity.Group.Id);
            OrderingUtils.SetOrder(group.Elements, entity, order);
            await _elementRepository.Update(group.Elements);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TElement entity = await Getter.GetElementWithGroupById(_elementRepository, entityId);
        if (entity.IsFavorite != isFavorite)
        {
            entity.IsFavorite = isFavorite;
            await _elementRepository.Update(entity);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TElement entity = await Getter.GetElementWithGroupById(_elementRepository, entityId);
        TGroup fromGroup = await _elementRepository.GetByGroupId(entity.Group.Id);
        TGroup toGroup = await _elementRepository.GetByGroupId(groupId);

        if (fromGroup.Id != toGroup.Id)
        {
            await Guard.CheckElementWithSameName(_elementRepository, toGroup.Id, entity.Id, entity.Name);
            int newOrder = await _elementRepository.GetMaxOrder(toGroup.Id) + 1;

            entity.Group = toGroup;
            toGroup.Elements.Add(entity);
            fromGroup.Elements.Remove(entity);
            
            OrderingUtils.Reorder(fromGroup.Elements);
            OrderingUtils.SetOrder(toGroup.Elements, entity, newOrder);

            await _elementRepository.Update(toGroup.Elements);
            await _elementRepository.Update(fromGroup.Elements);
            await _groupRepository.Update(toGroup);
            await _groupRepository.Update(fromGroup);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        TElement primaryEntity = await Getter.GetElementWithGroupById(_elementRepository, primaryId);
        TElement secondaryEntity = await Getter.GetElementWithGroupById(_elementRepository, secondaryId);

        if (primaryEntity.Id != secondaryEntity.Id)
        {
            ICollection<Account> accounts = await GetAccountsByEntity(_accountRepository, secondaryEntity);
            foreach (Account account in accounts)
            {
                AccountEntitySetter(primaryEntity, account);
                await _accountRepository.Update(account);
            }

            TGroup group = await _elementRepository.GetByGroupId(secondaryEntity.Group.Id);
            group.Elements.Remove(secondaryEntity);
            OrderingUtils.Reorder(group.Elements);

            await _elementRepository.Delete(secondaryEntity.Id);
            await _elementRepository.Update(group.Elements);
        }

        await unitOfWork.SaveChanges();
    }

    protected abstract Func<IAccountRepository, TElement, Task<ICollection<Account>>> GetAccountsByEntity { get; }
    protected abstract Action<TElement, Account> AccountEntitySetter { get; }
}