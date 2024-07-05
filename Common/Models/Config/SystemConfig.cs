using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Config;

public class SystemConfig : IEntity
{
    public Guid Id { get; set; }
    public string MainCurrencyIsoCode { get; set; }
    public DateTime MinDate { get; set; }
    public DateTime MaxDate { get; set; }
}