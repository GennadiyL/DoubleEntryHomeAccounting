using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class TemplateEntry : Entity
{
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    public Account Account { get; set; }
    public decimal Amount { get; set; }
}