using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ICurrencyRateRepository : IEntityRepository<CurrencyRate>
{
        Task<Currency> GetByPeriod(Guid currencyId, DateOnly startDate, DateOnly finishDate);
        Task<CurrencyRate> GetOnDate(Guid currencyId, DateOnly date);
}