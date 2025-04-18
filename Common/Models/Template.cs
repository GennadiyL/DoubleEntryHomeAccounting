using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Template : ElementReferenceEntity<TemplateGroup, Template>
{
    public List<TemplateEntry> Entries { get; } = new();
}