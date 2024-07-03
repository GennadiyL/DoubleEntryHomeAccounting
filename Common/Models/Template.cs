using GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models;

public class Template : ReferenceDataElementEntity<TemplateGroup>
{
    public List<TemplateEntry> Entries { get; } = new();
}