using GLSoft.DoubleEntryHomeAccounting.Common.Models;
using GLSoft.DoubleEntryHomeAccounting.Common.Params;
using GLSoft.DoubleEntryHomeAccounting.Common.Services.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Services;

public interface ICurrencyService : IOrderedService<Currency>, IFavoriteService<Currency>
{
    Task<Guid> Add(CurrencyParam param, decimal initialRate);
    Task Update(CurrencyParam param);
    Task Delete(string isoCode);
}