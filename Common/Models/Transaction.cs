using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Enums;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Transaction : Entity, ITrackedEntity
{
    public string TimeStamp { get; set; }
    public DateTime DateTime { get; set; }
    public TransactionState State { get; set; }
    public string Comment { get; set; }
    public List<TransactionEntry> Entries { get; set; } = new();
}