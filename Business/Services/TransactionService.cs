using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Model;
using GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Enums;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services;
using GLSoft.DoubleEntryHomeAccounting.Common.Utils.Check;

namespace Business.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ISystemConfigRepository _systemConfigRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public TransactionService(
        IUnitOfWorkFactory unitOfWorkFactory,
        ISystemConfigRepository systemConfigRepository,
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _systemConfigRepository = systemConfigRepository;
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Guid> Add(TransactionParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        await CheckInputTransactionParam(param);

        Transaction addedEntity = new Transaction();

        List<TransactionEntry> entries = await CreateEntries(param, addedEntity);
        decimal sumAmount = entries.Sum(e => e.Amount * e.Rate);    

        addedEntity.Comment = param.Comment;
        addedEntity.DateTime = param.DateTime;
        addedEntity.State = sumAmount == 0 ? param.State : TransactionState.NoValid;
        addedEntity.Entries.AddRange(entries);

        await _transactionRepository.Add(addedEntity);

        await unitOfWork.SaveChanges();

        return addedEntity.Id;
    }

    public async Task Update(Guid entityId, TransactionParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        await CheckInputTransactionParam(param);

        Transaction updatedEntity = await _transactionRepository.GetTransactionById(entityId)
                                    ?? throw new ArgumentNullException($"Transaction #{entityId} does not exist");
        List<TransactionEntry> oldEntries = updatedEntity.Entries;

        List<TransactionEntry> entries = await CreateEntries(param, updatedEntity);
        decimal sumAmount = entries.Sum(e => e.Amount * e.Rate);

        updatedEntity.Comment = param.Comment;
        updatedEntity.DateTime = param.DateTime;
        updatedEntity.State = sumAmount == 0 ? param.State : TransactionState.NoValid;
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(entries);
        oldEntries.ForEach(e => e.Transaction = null);

        await _transactionRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Transaction deletedTransaction = await _transactionRepository.GetTransactionById(entityId)
                                         ?? throw new ArgumentNullException($"Transaction #{entityId} does not exist");
        await _transactionRepository.Delete(deletedTransaction.Id);
    }

    public async Task DeleteTransactionList(List<Guid> transactionIds)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        Guard.CheckParamForNull(transactionIds);

        List<Transaction> deletedTransactions = new List<Transaction>();
        foreach (Guid transactionId in transactionIds)
        {
            Transaction deletedTransaction = await _transactionRepository.GetTransactionById(transactionId) 
                                             ?? throw new ArgumentNullException($"Transaction #{transactionId} does not exist");
            deletedTransactions.Add(deletedTransaction);
        }

        await _transactionRepository.Delete(deletedTransactions.Select(e => e.Id).ToList());

        await unitOfWork.SaveChanges();
    }

    private async Task CheckInputTransactionParam(TransactionParam param)
    {
        Guard.CheckParamForNull(param);

        if (DateOnly.FromDateTime(param.DateTime) < await _systemConfigRepository.GetMinDate() ||
            DateOnly.FromDateTime(param.DateTime) > await _systemConfigRepository.GetMaxDate())
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
        TransactionParam param, 
        Transaction transaction)
    {
        string mainCurrencyIsoCode = await _systemConfigRepository.GetMainCurrencyIsoCode();
 
        List<TransactionEntry> entries = new List<TransactionEntry>();
        foreach (TransactionEntryParam entryParam in param.Entries)
        {
            if (entryParam.Rate <= 0)
            {
                throw new ArgumentException("Currency rate must be more than 0");
            }

            Account account = await _accountRepository.GetById(entryParam.AccountId, Include<Account>.Create(e => e.Currency))
                              ?? throw new ArgumentNullException($"Account #{entryParam.AccountId} does not exist");

            if (account.Currency.IsoCode == mainCurrencyIsoCode && entryParam.Rate != 1)
            {
                throw new ArgumentException("Rate for main Currency must be 1");
            }

            TransactionEntry addedEntry = new TransactionEntry
            {
                Account = account,
                Rate = entryParam.Rate,
                Amount = entryParam.Amount,
                Transaction = transaction
            };
            entries.Add(addedEntry);

        }

        return entries;
    }
}