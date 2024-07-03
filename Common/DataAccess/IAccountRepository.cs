using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface IAccountRepository : IElementEntityRepository<Account>
{
    [Obsolete]
    Task LoadCurrency(Account account);

    Task<List<Account>> GetAccountsByCorrespondent(Correspondent correspondent);
    Task<List<Account>> GetAccountsByCategory(Category category);
    Task<List<Account>> GetAccountsByProject(Project project);
    Task<List<Account>> GetAccountsByCurrency(Currency currency);
}