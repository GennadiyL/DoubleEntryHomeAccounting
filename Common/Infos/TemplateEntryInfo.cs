using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class TemplateEntryInfo : Info
{
    public Guid TemplateId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}