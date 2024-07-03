using Common.DataAccess;
using Common.Infrastructure.Peaa;
using Common.Models;
using Common.Models.Enums;
using Common.Params;
using Common.Services;
using Common.Utils.Check;

namespace Business.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    //private readonly ISystemConfigRepository _systemConfigRepository;


    public TransactionService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Add(TransactionParam param)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITransactionRepository transactionRepository = await unitOfWork.GetRepository<ITransactionRepository>();
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        ISystemConfigRepository systemConfigRepository = await unitOfWork.GetRepository<ISystemConfigRepository>();
        

        CheckInputTransactionParam(systemConfigRepository, param);

        Transaction addedEntity = new Transaction();

        Tuple<List<TransactionEntry>, decimal> tuple = await CreateEntries(systemConfigRepository, accountRepository, param, addedEntity);
        List<TransactionEntry> entries = tuple.Item1;
        decimal sumAmount = tuple.Item2;    

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

        ITransactionRepository transactionRepository = await unitOfWork.GetRepository<ITransactionRepository>();
        IAccountRepository accountRepository = await unitOfWork.GetRepository<IAccountRepository>();
        ISystemConfigRepository systemConfigRepository = await unitOfWork.GetRepository<ISystemConfigRepository>();

        await CheckInputTransactionParam(systemConfigRepository, param);
        Transaction updatedEntity = await Getter.GetEntityById(transactionRepository.Get, entityId);

        Tuple<List<TransactionEntry>, decimal> tuple = await CreateEntries(systemConfigRepository, accountRepository, param, updatedEntity);
        List<TransactionEntry> entries = tuple.Item1;
        decimal sumAmount = tuple.Item2;

        updatedEntity.Comment = param.Comment;
        updatedEntity.DateTime = param.DateTime;
        updatedEntity.State = sumAmount == 0 ? param.State : TransactionState.NoValid;
        updatedEntity.Entries.Clear();
        updatedEntity.Entries.AddRange(entries);

        await transactionRepository.Update(updatedEntity);

        await unitOfWork.SaveChanges();
    }

    public async Task Delete(Guid entityId)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITransactionRepository transactionRepository = await unitOfWork.GetRepository<ITransactionRepository>();

        var deletedTransaction = await Getter.GetEntityById(transactionRepository.Get, entityId);
        await transactionRepository.Delete(deletedTransaction);
    }

    public async Task DeleteTransactionList(List<Guid> transactionIds)
    {
        using IUnitOfWork unitOfWork = _unitOfWorkFactory.Create();

        ITransactionRepository transactionRepository = await unitOfWork.GetRepository<ITransactionRepository>();

        Guard.CheckParamForNull(transactionIds);

        List<Transaction> deletedTransactions = new List<Transaction>();
        foreach (Guid transactionId in transactionIds)
        {
            var deletedTransaction = await Getter.GetEntityById(transactionRepository.Get, transactionId);
            deletedTransactions.Add(deletedTransaction);
        }

        await transactionRepository.DeleteList(deletedTransactions);

        await unitOfWork.SaveChanges();
    }

    private async Task CheckInputTransactionParam(ISystemConfigRepository systemConfigRepository, TransactionParam param)
    {
        Guard.CheckParamForNull(param);

        if (param.DateTime < await systemConfigRepository.GetMinDate() ||
            param.DateTime > await systemConfigRepository.GetMaxDate())
        {
            throw new ArgumentException("Data and Time is not in valid range");
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

    private async Task<Tuple<List<TransactionEntry>, decimal>> CreateEntries(
        ISystemConfigRepository systemConfigRepository, IAccountRepository accountRepository, TransactionParam param, Transaction transaction)
    {
        string mainCurrencyIsoCode = await systemConfigRepository.GetMainCurrencyIsoCode();
        decimal sumAmount = 0;

        List<TransactionEntry> entries = new List<TransactionEntry>();
        foreach (TransactionEntryParam entryParam in param.Entries)
        {
            if (entryParam.Rate <= 0)
            {
                throw new ArgumentException("Currency rate must be more than 0");
            }
            
            Account account = await Getter.GetEntityById(accountRepository.Get, entryParam.AccountId);
            await accountRepository.LoadCurrency(account);

            if (account.Currency.IsoCode == mainCurrencyIsoCode && entryParam.Rate != 1)
            {
                throw new ArgumentException("Rate for main Currency should be 1");
            }

            TransactionEntry addedEntry = new TransactionEntry
            {
                Account = account,
                Rate = entryParam.Rate,
                Amount = entryParam.Amount,
                Transaction = transaction
            };
            entries.Add(addedEntry);

            sumAmount += addedEntry.Amount * addedEntry.Rate;
        }

        return new Tuple<List<TransactionEntry>, decimal>(entries, sumAmount);
    }
}