using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Params;

public class CurrencyParam : IParam
{
    public string Code { get; set; }

    public string Symbol { get; set; }

    public string Name { get; set; }
}