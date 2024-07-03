namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ISystemConfigRepository
{
    Task<string> GetMainCurrencyIsoCode();
    Task<DateTime> GetMinDate();
    Task<DateTime> GetMaxDate();
}