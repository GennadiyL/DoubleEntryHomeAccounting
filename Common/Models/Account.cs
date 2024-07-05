using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Account : ReferenceDataElementEntity<AccountGroup, Account>
{
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public Guid CorrespondentId { get; set; }
    public Correspondent Correspondent { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
}