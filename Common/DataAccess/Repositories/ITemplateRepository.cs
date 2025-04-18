using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;

public interface ITemplateRepository : IElementRepository<TemplateGroup, Template>
{
    Task<Template> GetTemplateById(Guid id);
    Task<ICollection<TemplateEntry>> GetEntriesByAccountId(Guid accountId);
    Task<int> GetCountEntriesByAccountId(Guid accountId);
}