using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Params;

public class CurrencyRateParam : IParam
{
    public Guid CurrencyId { get; set; }

    public DateOnly Date { get; set; }

    public decimal Rate { get; set; }

    public string Comment { get; set; }
}