using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Ordering;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public AccountService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(AccountParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNullOrEmpty(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();
        ICategoryRepository categoryRepository = unitOfWork.GetRepository<ICategoryRepository>();
        ICorrespondentRepository correspondentRepository = unitOfWork.GetRepository<ICorrespondentRepository>();
        IProjectRepository projectRepository = unitOfWork.GetRepository<IProjectRepository>();

        AccountGroup group = await Guard.CheckAndGetEntityById(accountRepository.GetGroupWithElementsByGroupId, param.GroupId);

        await Guard.CheckElementWithSameName(accountRepository, group.Id, Guid.Empty, param.Name);

        Currency currency = await Guard.CheckAndGetEntityById(currencyRepository.GetById, param.CurrencyId);

        Category category =
            param.CategoryId == default ? default : await Guard.CheckAndGetEntityById(categoryRepository.GetById, param.CategoryId.Value);
        Correspondent correspondent =
            param.CorrespondentId == default ? default : await Guard.CheckAndGetEntityById(correspondentRepository.GetById, param.CorrespondentId.Value);
        Project project =
            param.ProjectId == default ? default : await Guard.CheckAndGetEntityById(projectRepository.GetById, param.ProjectId.Value);

        Account addedEntity = new Account
        {
            Id = Guid.NewGuid(),
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = group.Elements.GetMaxOrder() + 1,
            Currency = currency,
            CurrencyId = currency.Id,
            Category = category,
            CategoryId = category?.Id,
            Correspondent = correspondent,
            CorrespondentId = correspondent?.Id,
            Project = project,
            ProjectId = project?.Id,
            Group = group,
            GroupId = group.Id,
        };

        group.Elements.Add(addedEntity);

        await accountRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, AccountParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNullOrEmpty(param);

        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        ICategoryRepository categoryRepository = unitOfWork.GetRepository<ICategoryRepository>();
        ICorrespondentRepository correspondentRepository = unitOfWork.GetRepository<ICorrespondentRepository>();
        IProjectRepository projectRepository = unitOfWork.GetRepository<IProjectRepository>();

        Account updatedEntity = await Guard.CheckAndGetEntityById(accountRepository.GetById, entityId);

        AccountGroup group = await accountRepository.GetGroupWithElementsByGroupId(updatedEntity.GroupId);

        Guard.CheckEntityWithSameName(group.Elements, updatedEntity.Id, param.Name);

        Category category = null;
        Project project = null;
        Correspondent correspondent = null;
        if (updatedEntity.CategoryId != param.CategoryId)
        {
            category = param.CategoryId == default
                ? default
                : await Guard.CheckAndGetEntityById(categoryRepository.GetById, param.CategoryId.Value);
        }

        if (updatedEntity.CorrespondentId != param.CorrespondentId)
        {
            correspondent = param.CorrespondentId == default
                ? default
                : await Guard.CheckAndGetEntityById(correspondentRepository.GetById, param.CorrespondentId.Value);
        }

        if (updatedEntity.ProjectId != param.ProjectId)
        {
            project = param.ProjectId == default
                ? default
                : await Guard.CheckAndGetEntityById(projectRepository.GetById, param.ProjectId.Value);
        }

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        if (updatedEntity.CategoryId != param.CategoryId)
        {
            updatedEntity.Category = category;
            updatedEntity.CategoryId = category?.Id;
        }

        if (updatedEntity.CorrespondentId != param.CorrespondentId)
        {
            updatedEntity.Project = project;
            updatedEntity.ProjectId = project?.Id;
        }

        if (updatedEntity.ProjectId != param.ProjectId)
        {
            updatedEntity.Correspondent = correspondent;
            updatedEntity.CorrespondentId = correspondent?.Id;
        }

        await accountRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();

        Account deletedEntity = await Guard.CheckAndGetEntityById(accountRepository.GetById, entityId);

        AccountGroup group = await accountRepository.GetGroupWithElementsByGroupId(deletedEntity.Group.Id);

        if (await transactionRepository.GetCountEntriesByAccountId(deletedEntity.Id) > 0)
        {
            throw new ReferenceEntityException(typeof(Account), typeof(Transaction), deletedEntity.Id);
        }

        if (await templateRepository.GetCountEntriesByAccountId(deletedEntity.Id) > 0)
        {
            throw new ReferenceEntityException(typeof(Account), typeof(Template), deletedEntity.Id);
        }

        deletedEntity.Group = default;
        deletedEntity.GroupId = default;
        group.Elements.Remove(deletedEntity);
        group.Elements.Reorder();

        await accountRepository.Delete(deletedEntity.Id);
        await accountRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Account entity = await Guard.CheckAndGetEntityById(accountRepository.GetById, entityId);
        if (entity.Order == order)
        {
            return;
        }

        AccountGroup group = await accountRepository.GetGroupWithElementsByGroupId(entity.GroupId);

        group.Elements.SetOrder(entity, order);

        await accountRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Account entity = await Guard.CheckAndGetEntityById(accountRepository.GetById, entityId);
        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;

        await accountRepository.Update(entity);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid toGroupId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Account entity = await Guard.CheckAndGetEntityById(accountRepository.GetById, entityId);
        AccountGroup fromGroup = await accountRepository.GetGroupWithElementsByGroupId(entity.Group.Id);
        AccountGroup toGroup = await accountRepository.GetGroupWithElementsByGroupId(toGroupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(accountRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await accountRepository.GetMaxOrderInGroup(toGroup.Id) + 1;

        entity.Group = toGroup;
        entity.GroupId = toGroup.Id;
        entity.Order = newOrder;
        toGroup.Elements.Add(entity);

        fromGroup.Elements.Remove(entity);
        fromGroup.Elements.Reorder();

        await accountRepository.Update(toGroup.Elements);
        await accountRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid toElementId, Guid fromElementId)
    {
        IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();

        Account toAccount = await Guard.CheckAndGetEntityById(accountRepository.GetById, toElementId);
        Account fromAccount = await Guard.CheckAndGetEntityById(accountRepository.GetById, fromElementId);

        if (toAccount.Id == fromAccount.Id)
        {
            return;
        }

        if (toAccount.CurrencyId != fromAccount.CurrencyId)
        {
            throw new MismatchingCurrenciesException();
        }

        ICollection<TemplateEntry> templateEntries = await templateRepository.GetEntriesByAccountId(fromAccount.Id);
        IList<Template> templates = templateEntries.Select(e => e.Template).ToList();
        foreach (TemplateEntry templateEntry in templateEntries)
        {
            templateEntry.Account = toAccount;
        }

        ICollection<TransactionEntry> transactionEntries = await transactionRepository.GetEntriesByAccountId(fromAccount.Id);
        IList<Transaction> transactions = transactionEntries.Select(e => e.Transaction).ToList();
        foreach (TransactionEntry transactionEntry in transactionEntries)
        {
            transactionEntry.Account = toAccount;
        }

        AccountGroup group = await Guard.CheckAndGetEntityById(accountRepository.GetGroupWithElementsByGroupId, fromAccount.GroupId);
        fromAccount.Group = default;
        fromAccount.GroupId = default;
        group.Elements.Remove(fromAccount);
        group.Elements.Reorder();

        await templateRepository.Update(templates);
        await transactionRepository.Update(transactions);
        await accountRepository.Delete(fromAccount.Id);
        await accountRepository.Update(group.Elements);
        
        await unitOfWork.SaveChanges();
    }
}