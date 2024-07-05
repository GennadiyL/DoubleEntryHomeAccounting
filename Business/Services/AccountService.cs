using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
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
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountGroupRepository accountGroupRepository = unitOfWork.GetRepository<IAccountGroupRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();
        ICategoryRepository categoryRepository = unitOfWork.GetRepository<ICategoryRepository>();
        ICorrespondentRepository correspondentRepository = unitOfWork.GetRepository<ICorrespondentRepository>();
        IProjectRepository projectRepository = unitOfWork.GetRepository<IProjectRepository>();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        AccountGroup group = await Getter.GetEntityById(accountGroupRepository, param.GroupId);
        await Guard.CheckElementWithSameName(accountRepository, group.Id, Guid.Empty, param.Name);

        Account addedEntity = new Account
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await accountRepository.GetMaxOrder(group.Id) + 1,
            Currency = await Getter.GetEntityById(currencyRepository, param.CurrencyId),
            Category = param.CategoryId == default ? default 
                : await Getter.GetEntityById(categoryRepository, param.CategoryId.Value),
            Project = param.ProjectId == default ? default 
                : await Getter.GetEntityById(projectRepository, param.ProjectId.Value),
            Correspondent = param.CorrespondentId == default ? default 
                : await Getter.GetEntityById(correspondentRepository, param.CorrespondentId.Value),
        };

        addedEntity.Group = group;
        group.Elements.Add(addedEntity);

        await accountRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, AccountParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ICategoryRepository categoryRepository = unitOfWork.GetRepository<ICategoryRepository>();
        ICorrespondentRepository correspondentRepository = unitOfWork.GetRepository<ICorrespondentRepository>();
        IProjectRepository projectRepository = unitOfWork.GetRepository<IProjectRepository>();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        Account updatedEntity = await Getter.GetEntityById(accountRepository, entityId);
        await Guard.CheckElementWithSameName(accountRepository, updatedEntity.GroupId, entityId, param.Name);

        Category category = param.CategoryId == default ? default 
            : await Getter.GetEntityById(categoryRepository, param.CategoryId.Value);
        Project project = param.ProjectId == default ? default 
            : await Getter.GetEntityById(projectRepository, param.ProjectId.Value);
        Correspondent correspondent = param.CorrespondentId == default ? default 
            : await Getter.GetEntityById(correspondentRepository, param.CorrespondentId.Value);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        updatedEntity.Category = category;
        updatedEntity.Project = project;
        updatedEntity.Correspondent = correspondent;

        await accountRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();

        Account deletedEntity = await Getter.GetEntityById(accountRepository, entityId);
        if (await transactionRepository.GetCountEntriesByAccountId(deletedEntity.Id) > 0)
        {
            throw new Exception("Account cannot be delete, it contains transaction.");
        }
        if (await templateRepository.GetCountEntriesByAccountId(deletedEntity.Id) > 0)
        {
            throw new Exception("Account cannot be delete, it contains template.");
        }

        AccountGroup group = await accountRepository.GetByGroupId(deletedEntity.Group.Id);

        group.Elements.Remove(deletedEntity);
        OrderingUtils.Reorder(group.Elements);

        await accountRepository.Delete(deletedEntity.Id);
        await accountRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Account entity = await Getter.GetEntityById(accountRepository, entityId);
        if (entity.Order == order)
        {
            return;
        }

        AccountGroup group = await accountRepository.GetByGroupId(entity.Group.Id);
        OrderingUtils.SetOrder(group.Elements, entity, order);

        await accountRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Account entity = await Getter.GetEntityById(accountRepository, entityId);
        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;

        await accountRepository.Update(entity);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();

        Account entity = await Getter.GetEntityById(accountRepository, entityId);
        AccountGroup fromGroup = await accountRepository.GetByGroupId(entity.Group.Id);
        AccountGroup toGroup = await accountRepository.GetByGroupId(groupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(accountRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await accountRepository.GetMaxOrder(toGroup.Id) + 1;

        fromGroup.Elements.Remove(entity);
        entity.Group = toGroup;
        toGroup.Elements.Add(entity);

        OrderingUtils.Reorder(fromGroup.Elements);
        OrderingUtils.SetOrder(toGroup.Elements, entity, newOrder);

        await accountRepository.Update(toGroup.Elements);
        await accountRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ITemplateRepository templateRepository = unitOfWork.GetRepository<ITemplateRepository>();
        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();

        Account primaryAccount = await Getter.GetEntityById(accountRepository, primaryId);
        Account secondaryAccount = await Getter.GetEntityById(accountRepository, secondaryId);

        if (primaryAccount.Id == secondaryAccount.Id)
        {
            return;
        }
        if (primaryAccount.CurrencyId != secondaryAccount.CurrencyId)
        {
            throw new ArgumentException("Can't combine accounts with different currencies");
        }

        ICollection<TemplateEntry> templateEntries =
            await templateRepository.GetEntriesByAccountId(secondaryAccount.Id);
        IList<Template> templates = templateEntries.Select(e => e.Template).ToList();
        foreach (TemplateEntry templateEntry in templateEntries)
        {
            templateEntry.Account = primaryAccount;
        }

        ICollection<TransactionEntry> transactionEntries =
            await transactionRepository.GetEntriesByAccountId(secondaryAccount.Id);
        IList<Transaction> transactions = transactionEntries.Select(e => e.Transaction).ToList();
        foreach (TransactionEntry transactionEntry in transactionEntries)
        {
            transactionEntry.Account = primaryAccount;
        }

        AccountGroup secondaryGroup = await accountRepository.GetByGroupId(secondaryAccount.GroupId);
        secondaryGroup.Elements.Remove(secondaryAccount);
        OrderingUtils.Reorder(secondaryGroup.Elements);

        await templateRepository.Update(templates);
        await transactionRepository.Update(transactions);
        await accountRepository.Delete(secondaryAccount.Id);
        await accountRepository.Update(secondaryGroup.Elements);

        await unitOfWork.SaveChanges();
    }
}