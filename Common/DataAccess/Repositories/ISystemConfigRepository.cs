using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Config;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories;

public interface ISystemConfigRepository : IEntityRepository<SystemConfig>
{
    Task<string> GetMainCurrencyIsoCode();
    Task<DateOnly> GetMinDate();
    Task<DateOnly> GetMaxDate();
}