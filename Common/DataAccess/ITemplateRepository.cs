using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ITemplateRepository : IElementEntityRepository<Template>
{
    Task<List<TemplateEntry>> GetEntriesByAccount(Account account);
    Task<int> GetTemplateEntriesCount(Guid accountId);
}