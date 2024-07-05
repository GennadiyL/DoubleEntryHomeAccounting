using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ITemplateRepository : IElementEntityRepository<TemplateGroup, Template>
{
    Task<Template> GetTemplateById(Guid id);
    Task<ICollection<TemplateEntry>> GetEntriesByAccountId(Guid accountId);
    Task<int> GetCountEntriesByAccountId(Guid accountId);
}