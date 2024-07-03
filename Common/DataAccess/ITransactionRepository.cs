using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ITransactionRepository : IEntityRepository<Transaction>
{
    Task<List<TransactionEntry>> GetEntriesByAccount(Account account);
    Task<int> GetTransactionEntriesCount(Guid accountId);
}