using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ICurrencyRateRepository : IEntityRepository<CurrencyRate>
{
        Task<Currency> GetRatesByPeriod(Guid currencyId, DateOnly startDate, DateOnly finishDate);
        Task<CurrencyRate> GetRateOnDate(Guid currencyId, DateOnly date);
}