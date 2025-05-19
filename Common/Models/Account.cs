using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Account : ElementEntity<AccountGroup, Account>
{
    public Currency Currency { get; set; }
    public Guid CurrencyId { get; set; }
    public Category Category { get; set; }
    public Guid? CategoryId { get; set; }
    public Correspondent Correspondent { get; set; }
    public Guid? CorrespondentId { get; set; }
    public Project Project { get; set; }
    public Guid? ProjectId { get; set; }
}