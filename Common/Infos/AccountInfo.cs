using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class AccountInfo : ElementInfo
{
    public Guid CurrencyId { get; set; }
    public Guid? CorrespondentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? ProjectId { get; set; }
}