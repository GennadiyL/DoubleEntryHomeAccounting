using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class CurrencyInfo : Info, IFavoriteInfo, IOrderedInfo
{
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public List<Guid> RatesIds { get; } = new();
    public bool IsFavorite { get; set; }
    public int Order { get; set; }
}