using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Template : ElementEntity<TemplateGroup, Template>
{
    public List<TemplateEntry> Entries { get; } = new();
}