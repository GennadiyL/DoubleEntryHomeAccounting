using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;

public interface ICategoryRepository : IElementRepository<CategoryGroup, Category>
{
}