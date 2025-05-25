using GLSoft.DoubleEntryHomeAccounting.Common.Params;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services;

public interface ICurrencyRateService
{
    Task<Guid> AddOrUpdate(CurrencyRateParam param);
    Task Delete(Guid currencyId, DateOnly fromDate, DateOnly toDate);
}