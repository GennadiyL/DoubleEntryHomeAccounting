using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ICurrencyRepository : IEntityRepository<Currency>
{
    Task<Currency> GetByIsoCode(string isoCode);
    Task<int> GetMaxOrder();
    Task<int> GetCount();
    Task<List<Currency>> GetList();
}