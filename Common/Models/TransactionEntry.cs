using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class TransactionEntry : Entity
{
    public Transaction Transaction { get; set; }
    public Account Account { get; set; }
    public decimal Amount { get; set; }
    public decimal Rate { get; set; }
}