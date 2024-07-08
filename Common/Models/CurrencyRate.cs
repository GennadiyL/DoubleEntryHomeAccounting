using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class CurrencyRate : Entity, ITrackedEntity
{
    public DateTime Original { get; set; }
    public DateTime Current { get; set; }
    public bool IsDeleted { get; set; }
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public DateOnly Date { get; set; }
    public decimal Rate { get; set; }
    public string Comment { get; set; }
}