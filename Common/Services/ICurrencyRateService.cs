using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services;

public interface ICurrencyRateService
{
    Task<Guid> AddOrUpdate(CurrencyRateParam param);
    Task Delete(Guid currencyId, DateOnly fromDate, DateOnly toDate);
}