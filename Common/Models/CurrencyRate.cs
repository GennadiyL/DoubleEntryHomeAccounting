using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class CurrencyRate : Entity, ITrackedEntity
{
    public string TimeStamp { get; set; }
    public Currency Currency { get; set; }
    public DateTime Date { get; set; }
    public decimal Rate { get; set; }
    public string Comment { get; set; }
}