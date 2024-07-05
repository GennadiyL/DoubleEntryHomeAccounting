namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface ISystemConfigRepository
{
    Task<string> GetMainCurrencyIsoCode();
    Task<DateOnly> GetMinDate();
    Task<DateOnly> GetMaxDate();
}