using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;

public interface ICurrencyRepository : IEntityRepository<Currency>
{
    Task<Currency> GetByIsoCode(string isoCode);
    Task<int> GetMaxOrder();
}