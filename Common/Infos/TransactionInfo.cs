using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Enums;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class TransactionInfo : Info
{
    public DateTime DateTime { get; set; }
    public TransactionState State { get; set; }
    public string Comment { get; set; }
    public List<Guid> EntriesIds { get; } = new();
}