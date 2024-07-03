using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Account : ReferenceDataElementEntity<AccountGroup>
{
    public Currency Currency { get; set; }
    public Category Category { get; set; }
    public Correspondent Correspondent { get; set; }
    public Project Project { get; set; }
}