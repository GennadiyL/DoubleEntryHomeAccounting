using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Enums;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;

namespace GLSoft.DoubleEntryHomeAccounting.Business.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public TransactionService(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Guid> Add(TransactionParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ISystemConfigRepository systemConfigRepository = unitOfWork.GetRepository<ISystemConfigRepository>();
        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        await CheckInputTransactionParam(systemConfigRepository, param);

        Transaction addedEntity = new Transaction();

        List<TransactionEntry> entries = await CreateEntries(systemConfigRepository, currencyRepository, accountRepository, param, addedEntity);
        decimal sumAmount = entries.Sum(e => e.Amount * e.Rate);

        addedEntity.Comment = param.Comment;
        addedEntity.DateTime = param.DateTime;
        addedEntity.State = sumAmount == 0 ? param.State : TransactionState.NoValid;
        addedEntity.Entries.AddRange(entries);

        await transactionRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TransactionParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ISystemConfigRepository systemConfigRepository = unitOfWork.GetRepository<ISystemConfigRepository>();
        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();
        IAccountRepository accountRepository = unitOfWork.GetRepository<IAccountRepository>();
        ICurrencyRepository currencyRepository = unitOfWork.GetRepository<ICurrencyRepository>();

        await CheckInputTransactionParam(systemConfigRepository, param);

        Transaction updatedEntity = await transactionRepository.GetTransactionById(entityId)
                                    ?? throw new ArgumentNullException($"Transaction #{entityId} does not exist");

        List<TransactionEntry> oldEntries = updatedEntity.Entries;
        List<TransactionEntry> newEntries = await CreateEntries(systemConfigRepository, currencyRepository, accountRepository, param, updatedEntity);
        decimal totalAmount = newEntries.Sum(e => e.Amount * e.Rate);

        updatedEntity.Comment = param.Comment;
        updatedEntity.DateTime = param.DateTime;
        updatedEntity.State = totalAmount == 0 ? param.State : TransactionState.NoValid;
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(newEntries);
        oldEntries.ForEach(e => e.Transaction = null);

        await transactionRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();

        Transaction deletedTransaction = await transactionRepository.GetTransactionById(entityId)
                                         ?? throw new ArgumentNullException($"Transaction #{entityId} does not exist");
        await transactionRepository.Delete(deletedTransaction.Id);
    }

    public async Task DeleteTransactionList(List<Guid> transactionIds)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITransactionRepository transactionRepository = unitOfWork.GetRepository<ITransactionRepository>();

        Guard.CheckParamForNull(transactionIds);

        List<Transaction> deletedTransactions = new List<Transaction>();
        foreach (Guid transactionId in transactionIds)
        {
            Transaction deletedTransaction = await transactionRepository.GetTransactionById(transactionId)
                                             ?? throw new ArgumentNullException($"Transaction #{transactionId} does not exist");
            deletedTransactions.Add(deletedTransaction);
        }

        await transactionRepository.Delete(deletedTransactions.Select(e => e.Id).ToList());

        await unitOfWork.SaveChanges();
    }

    private async Task CheckInputTransactionParam(ISystemConfigRepository systemConfigRepository, TransactionParam param)
    {
        Guard.CheckParamForNull(param);

        if (DateOnly.FromDateTime(param.DateTime) < await systemConfigRepository.GetMinDate() ||
            DateOnly.FromDateTime(param.DateTime) > await systemConfigRepository.GetMaxDate())
        {
            throw new ArgumentException("Data and Time out of ragne.");
        }

        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new ArgumentException("Invalid amount of Transaction Entries: amount must be more than 1");
        }

        if (!Enum.GetValues<TransactionState>().Contains(param.State))
        {
            throw new ArgumentException("Invalid transaction state");
        }
    }

    private async Task<List<TransactionEntry>> CreateEntries(
        ISystemConfigRepository systemConfigRepository,
        ICurrencyRepository currencyRepository,
        IAccountRepository accountRepository,
        TransactionParam param,
        Transaction transaction)
    {
        string mainCurrencyIsoCode = await systemConfigRepository.GetMainCurrencyIsoCode();
        Currency mainCurrency = await currencyRepository.GetByIsoCode(mainCurrencyIsoCode);

        List<TransactionEntry> entries = new List<TransactionEntry>();
        foreach (TransactionEntryParam entryParam in param.Entries)
        {
            if (entryParam.Rate <= 0)
            {
                throw new ArgumentException("Currency rate must be more than 0");
            }

            Account account = await Getter.GetEntityById(accountRepository, entryParam.AccountId);

            if (account.CurrencyId == mainCurrency.Id && entryParam.Rate != 1)
            {
                throw new ArgumentException("Rate for main Currency must be 1");
            }

            TransactionEntry addedEntry = new TransactionEntry
            {
                Account = account,
                AccountId = account.Id,
                Rate = entryParam.Rate,
                Amount = entryParam.Amount,
                Transaction = transaction,
                TransactionId = transaction.Id
            };
            entries.Add(addedEntry);

        }

        return entries;
    }
}