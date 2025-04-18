using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;

public interface IAccountRepository : IElementRepository<AccountGroup, Account>
{
    Task<ICollection<Account>> GetByCorrespondentId(Guid correspondentId);
    Task<ICollection<Account>> GetByCategoryId(Guid categoryId);
    Task<ICollection<Account>> GetByProjectId(Guid projectId);
    Task<ICollection<Account>> GetByCurrencyId(Guid currencyId);
}