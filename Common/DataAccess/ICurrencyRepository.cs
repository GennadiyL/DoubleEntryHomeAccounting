using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ICurrencyRepository : IEntityRepository<Currency>
{
    Task<Currency> GetByIsoCode(string isoCode);
    Task<int> GetMaxOrder();
}