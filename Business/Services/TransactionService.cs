using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;
using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
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

        Transaction updatedEntity = await Getter.GetEntityById(transactionRepository.GetTransactionById, entityId);

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

        Transaction deletedTransaction = await Getter.GetEntityById(transactionRepository.GetTransactionById, entityId); 
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
            Transaction deletedTransaction = await Getter.GetEntityById(transactionRepository.GetTransactionById, transactionId);
            deletedTransactions.Add(deletedTransaction);
        }

        await transactionRepository.Delete(deletedTransactions.Select(e => e.Id).ToList());

        await unitOfWork.SaveChanges();
    }

    private async Task CheckInputTransactionParam(ISystemConfigRepository systemConfigRepository, TransactionParam param)
    {
        Guard.CheckParamForNull(param);
        Guard.CheckEnumeration(param.State);

        DateOnly minDate = await systemConfigRepository.GetMinDate();
        DateOnly maxDate = await systemConfigRepository.GetMaxDate();
        DateOnly currentDate = DateOnly.FromDateTime(param.DateTime);
        if (currentDate < minDate || currentDate > maxDate)
        {
            throw new DateTimeOutOfRangeException(minDate, maxDate, param.DateTime);
        }

        if (param.Entries == null || param.Entries.Count < 2)
        {
            throw new EntriesAmountException();
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
                throw new InvalidCurrencyRateException(entryParam.Rate);
            }

            Account account = await Getter.GetEntityById(g => accountRepository.GetById(g), entryParam.AccountId);

            if (account.CurrencyId == mainCurrency.Id && entryParam.Rate != 1)
            {
                throw new InvalidCurrencyRateException(entryParam.Rate);
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