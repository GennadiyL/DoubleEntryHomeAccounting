using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using Transaction = GLSoft.DoubleEntryHomeAccounting.Common.Models.Transaction;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;

public interface ITransactionRepository : IEntityRepository<Transaction>
{
    Task<Transaction> GetTransactionById(Guid id);
    Task<ICollection<TransactionEntry>> GetEntriesByAccountId(Guid accountId);
    Task<int> GetCountEntriesByAccountId(Guid accountId);
}