using Common.DataAccess;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Params;
using Common.Services;
using Common.Utils.Check;
using Common.Utils.Ordering;

namespace Business.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public AccountService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Add(AccountParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);
        
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        ICurrencyRepository currencyRepository = await unitOfWork.GetRepository<ICurrencyRepository>();
        ICategoryRepository categoryRepository = await unitOfWork.GetRepository<ICategoryRepository>();
        ICorrespondentRepository correspondentRepository = await unitOfWork.GetRepository<ICorrespondentRepository>();
        IProjectRepository projectRepository = await unitOfWork.GetRepository<IProjectRepository>();

        AccountSubGroup subGroup = await Getter.GetEntityById(accountSubGroupRepository.Get, param.ParentId);
        await Guard.CheckEntityWithSameName(accountRepository, subGroup.Id, Guid.Empty, param.Name);

        Account addedEntity = new Account
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await accountRepository.GetMaxOrder(subGroup.Id) + 1,
            Currency = await Getter.GetEntityById(currencyRepository.Get, param.CurrencyId),
            Category = param.CategoryId == default ? default : await Getter.GetEntityById(categoryRepository.Get, param.CategoryId.Value),
            Project = param.ProjectId == default ? default : await Getter.GetEntityById(projectRepository.Get, param.ProjectId.Value),
            Correspondent = param.CorrespondentId == default ? default : await Getter.GetEntityById(correspondentRepository.Get, param.CorrespondentId.Value),
        };

        subGroup.Children.Add(addedEntity);
        addedEntity.Parent = subGroup;
        await accountRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, AccountParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        ICategoryRepository categoryRepository = await unitOfWork.GetRepository<ICategoryRepository>();
        ICorrespondentRepository correspondentRepository = await unitOfWork.GetRepository<ICorrespondentRepository>();
        IProjectRepository projectRepository = await unitOfWork.GetRepository<IProjectRepository>();

        Account updatedEntity = await Getter.GetEntityById(accountRepository.Get, entityId);
        await accountRepository.LoadParent(updatedEntity);
        AccountSubGroup parent = updatedEntity.Parent;
        await Guard.CheckEntityWithSameName(accountRepository, parent.Id, entityId, param.Name);

        Category category = param.CategoryId == default ? default : await Getter.GetEntityById(categoryRepository.Get, param.CategoryId.Value);
        Project project = param.ProjectId == default ? default : await Getter.GetEntityById(projectRepository.Get, param.ProjectId.Value);
        Correspondent correspondent = param.CorrespondentId == default ? default : await Getter.GetEntityById(correspondentRepository.Get, param.CorrespondentId.Value);

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

        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        ITemplateRepository templateRepository = await unitOfWork.GetRepository<ITemplateRepository>();
        ITransactionRepository transactionRepository = await unitOfWork.GetRepository<ITransactionRepository>();

        Account deletedEntity = await Getter.GetEntityById(accountRepository.Get, entityId);
        if (await transactionRepository.GetTransactionEntriesCount(deletedEntity.Id) > 0)
        {
            throw new Exception("Account cannot be delete, it contains transaction.");
        }
        if (await templateRepository.GetTemplateEntriesCount(deletedEntity.Id) > 0)
        {
            throw new Exception("Account cannot be delete, it contains template.");
        }

        await accountRepository.LoadParent(deletedEntity);
        AccountSubGroup subGroup = deletedEntity.Parent;
        await accountSubGroupRepository.LoadChildren(subGroup);

        subGroup.Children.Remove(deletedEntity);
        await accountRepository.Delete(deletedEntity);

        OrderingUtils.Reorder(subGroup.Children);
        await accountRepository.UpdateList(subGroup.Children);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        Account entity = await Getter.GetEntityById(accountRepository.Get, entityId);
        if (entity.IsFavorite != isFavorite)
        {
            entity.IsFavorite = isFavorite;
            await accountRepository.Update(entity);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();

        Account entity = await Getter.GetEntityById(accountRepository.Get, entityId);
        if (entity.Order != order)
        {
            await accountRepository.LoadParent(entity);
            AccountSubGroup subGroup = entity.Parent;
            await accountSubGroupRepository.LoadChildren(subGroup);

            OrderingUtils.SetOrder(subGroup.Children, entity, order);
            await accountRepository.UpdateList(subGroup.Children);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherParent(Guid entityId, Guid parentId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();

        Account entity = await Getter.GetEntityById(accountRepository.Get, entityId);
        await accountRepository.LoadParent(entity);
        AccountSubGroup fromParent = entity.Parent;
        AccountSubGroup toParent = await Getter.GetEntityById(accountSubGroupRepository.Get, parentId);

        if (fromParent.Id != toParent.Id)
        {
            await Guard.CheckEntityWithSameName(accountRepository, toParent.Id, entity.Id, entity.Name);

            await accountSubGroupRepository.LoadChildren(fromParent);

            fromParent.Children.Remove(entity);
            toParent.Children.Add(entity);
            entity.Parent = toParent;
            entity.Order = await accountRepository.GetMaxOrder(toParent.Id) + 1;

            await accountRepository.Update(entity);

            OrderingUtils.Reorder(fromParent.Children);
            await accountRepository.UpdateList(fromParent.Children);
        }

        await unitOfWork.SaveChanges();
    }

    public async Task CombineTwoEntities(Guid primaryId, Guid secondaryId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        IAccountSubGroupRepository accountSubGroupRepository = await unitOfWork.GetRepository<IAccountSubGroupRepository>();
        ITemplateRepository templateRepository = await unitOfWork.GetRepository<ITemplateRepository>();
        ITransactionRepository transactionRepository = await unitOfWork.GetRepository<ITransactionRepository>();

        Account primaryAccount = await Getter.GetEntityById(accountRepository.Get, primaryId);
        Account secondaryAccount = await Getter.GetEntityById(accountRepository.Get, secondaryId);

        if (primaryAccount.Id != secondaryAccount.Id)
        {
            await accountRepository.LoadCurrency(primaryAccount);
            await accountRepository.LoadCurrency(secondaryAccount);
            if (primaryAccount.Currency.Id != secondaryAccount.Currency.Id)
            {
                throw new ArgumentException("Can't combine accounts with different currencies");
            }

            List<TemplateEntry> templates = await templateRepository.GetEntriesByAccount(secondaryAccount);
            foreach (TemplateEntry templateEntry in templates)
            {
                templateEntry.Account = primaryAccount;
                await templateRepository.Update(templateEntry.Template);
            }

            List<TransactionEntry> transactions = await transactionRepository.GetEntriesByAccount(secondaryAccount);
            foreach (TransactionEntry transactionEntry in transactions)
            {
                transactionEntry.Account = primaryAccount;
                await transactionRepository.Update(transactionEntry.Transaction);
            }

            await accountRepository.LoadParent(secondaryAccount);
            AccountSubGroup secondarySubGroup = secondaryAccount.Parent;
            await accountSubGroupRepository.LoadChildren(secondarySubGroup);

            secondarySubGroup.Children.Remove(secondaryAccount);
            await accountRepository.Delete(secondaryAccount);

            OrderingUtils.Reorder(secondarySubGroup.Children);
            await accountRepository.UpdateList(secondarySubGroup.Children);
        }

        await unitOfWork.SaveChanges();
    }
}