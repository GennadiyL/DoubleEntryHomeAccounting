using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Currency : Entity, ITrackedEntity, IFavoriteEntity, IOrderedEntity
{
    public string TimeStamp { get; set; }
    public string IsoCode { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public bool IsFavorite { get; set; }
    public int Order { get; set; }
    public List<CurrencyRate> Rates { get; } = new();
}