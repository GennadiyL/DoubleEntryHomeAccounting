using GLSoft.DoubleEntryHomeAccounting.Common.Models.Enums;
using GLSoft.DoubleEntryHomeAccounting.Common.Params.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Params;

public class TransactionParam : IParam
{
    public DateTime DateTime { get; set; }
    public TransactionState State { get; set; }
    public string Comment { get; set; }
    public List<TransactionEntryParam> Entries { get; } = new();
}