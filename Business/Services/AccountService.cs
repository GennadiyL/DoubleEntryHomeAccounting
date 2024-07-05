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
    private readonly IAccountGroupRepository _accountGroupRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICorrespondentRepository _correspondentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AccountService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IAccountGroupRepository accountGroupRepository,
        IAccountRepository accountRepository,
        ICurrencyRepository currencyRepository,
        ICategoryRepository categoryRepository,
        ICorrespondentRepository correspondentRepository,
        IProjectRepository projectRepository,
        ITemplateRepository templateRepository,
        ITransactionRepository transactionRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _accountGroupRepository = accountGroupRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _categoryRepository = categoryRepository;
        _correspondentRepository = correspondentRepository;
        _projectRepository = projectRepository;
        _templateRepository = templateRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Guid> Add(AccountParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        AccountGroup group = await Getter.GetEntityById(_accountGroupRepository, param.GroupId);
        await Guard.CheckElementWithSameName(_accountRepository, group.Id, Guid.Empty, param.Name);

        Account addedEntity = new Account
        {
            Name = param.Name,
            Description = param.Description,
            IsFavorite = param.IsFavorite,
            Order = await _accountRepository.GetMaxOrder(group.Id) + 1,
            Currency = await Getter.GetEntityById(_currencyRepository, param.CurrencyId),
            Category = param.CategoryId == default ? default : await Getter.GetEntityById(_categoryRepository, param.CategoryId.Value),
            Project = param.ProjectId == default ? default : await Getter.GetEntityById(_projectRepository, param.ProjectId.Value),
            Correspondent = param.CorrespondentId == default ? default : await Getter.GetEntityById(_correspondentRepository, param.CorrespondentId.Value),
        };

        addedEntity.Group = group;
        group.Elements.Add(addedEntity);

        await _accountRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, AccountParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(param);
        Guard.CheckParamNameForNull(param);

        Account updatedEntity = await Getter.GetEntityById(_accountRepository, entityId);
        await Guard.CheckElementWithSameName(_accountRepository, updatedEntity.GroupId, entityId, param.Name);

        Category category = param.CategoryId == default ? default : await Getter.GetEntityById(_categoryRepository, param.CategoryId.Value);
        Project project = param.ProjectId == default ? default : await Getter.GetEntityById(_projectRepository, param.ProjectId.Value);
        Correspondent correspondent = param.CorrespondentId == default ? default : await Getter.GetEntityById(_correspondentRepository, param.CorrespondentId.Value);

        updatedEntity.Name = param.Name;
        updatedEntity.Description = param.Description;
        updatedEntity.IsFavorite = param.IsFavorite;

        updatedEntity.Category = category;
        updatedEntity.Project = project;
        updatedEntity.Correspondent = correspondent;

        await _accountRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Account deletedEntity = await Getter.GetEntityById(_accountRepository, entityId);
        if (await _transactionRepository.GetCountEntriesByAccountId(deletedEntity.Id) > 0)
        {
            throw new Exception("Account cannot be delete, it contains transaction.");
        }
        if (await _templateRepository.GetCountEntriesByAccountId(deletedEntity.Id) > 0)
        {
            throw new Exception("Account cannot be delete, it contains template.");
        }

        AccountGroup group = await _accountRepository.GetByGroupId(deletedEntity.Group.Id);

        group.Elements.Remove(deletedEntity);
        OrderingUtils.Reorder(group.Elements);

        await _accountRepository.Delete(deletedEntity.Id);
        await _accountRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetOrder(Guid entityId, int order)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Account entity = await Getter.GetEntityById(_accountRepository, entityId);
        if (entity.Order == order)
        {
            return;
        }

        AccountGroup group = await _accountRepository.GetByGroupId(entity.Group.Id);
        OrderingUtils.SetOrder(group.Elements, entity, order);

        await _accountRepository.Update(group.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task SetFavoriteStatus(Guid entityId, bool isFavorite)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Account entity = await Getter.GetEntityById(_accountRepository, entityId);
        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;

        await _accountRepository.Update(entity);

        await unitOfWork.SaveChanges();
    }

    public async Task MoveToAnotherGroup(Guid entityId, Guid groupId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Account entity = await Getter.GetEntityById(_accountRepository, entityId);
        AccountGroup fromGroup = await _accountRepository.GetByGroupId(entity.Group.Id);
        AccountGroup toGroup = await _accountRepository.GetByGroupId(groupId);

        if (fromGroup.Id == toGroup.Id)
        {
            return;
        }

        await Guard.CheckElementWithSameName(_accountRepository, toGroup.Id, entity.Id, entity.Name);
        int newOrder = await _accountRepository.GetMaxOrder(toGroup.Id) + 1;

        fromGroup.Elements.Remove(entity);
        entity.Group = toGroup;
        toGroup.Elements.Add(entity);

        OrderingUtils.Reorder(fromGroup.Elements);
        OrderingUtils.SetOrder(toGroup.Elements, entity, newOrder);

        await _accountRepository.Update(toGroup.Elements);
        await _accountRepository.Update(fromGroup.Elements);

        await unitOfWork.SaveChanges();
    }

    public async Task CombineElements(Guid primaryId, Guid secondaryId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Account primaryAccount = await Getter.GetEntityById(_accountRepository, primaryId);
        Account secondaryAccount = await Getter.GetEntityById(_accountRepository, secondaryId);

        if (primaryAccount.Id == secondaryAccount.Id)
        {
            return;
        }
        if (primaryAccount.CurrencyId != secondaryAccount.CurrencyId)
        {
            throw new ArgumentException("Can't combine accounts with different currencies");
        }

        ICollection<TemplateEntry> templateEntries =
            await _templateRepository.GetEntriesByAccountId(secondaryAccount.Id);
        IList<Template> templates = templateEntries.Select(e => e.Template).ToList();
        foreach (TemplateEntry templateEntry in templateEntries)
        {
            templateEntry.Account = primaryAccount;
        }

        ICollection<TransactionEntry> transactionEntries =
            await _transactionRepository.GetEntriesByAccountId(secondaryAccount.Id);
        IList<Transaction> transactions = transactionEntries.Select(e => e.Transaction).ToList();
        foreach (TransactionEntry transactionEntry in transactionEntries)
        {
            transactionEntry.Account = primaryAccount;
        }

        AccountGroup secondaryGroup = await _accountRepository.GetByGroupId(secondaryAccount.GroupId);
        secondaryGroup.Elements.Remove(secondaryAccount);
        OrderingUtils.Reorder(secondaryGroup.Elements);

        await _templateRepository.Update(templates);
        await _transactionRepository.Update(transactions);
        await _accountRepository.Delete(secondaryAccount.Id);
        await _accountRepository.Update(secondaryGroup.Elements);

        await unitOfWork.SaveChanges();
    }
}