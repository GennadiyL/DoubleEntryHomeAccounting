using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class TemplateInfo : ElementInfo
{
    public List<Guid> EntriesIds { get; } = new();
}