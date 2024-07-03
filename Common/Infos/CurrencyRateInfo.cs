using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class CurrencyRateInfo : Info
{
    public Guid CurrencyId { get; set; }
    public DateTime Date { get; set; }
    public decimal Rate { get; set; }
    public string Comment { get; set; }
}