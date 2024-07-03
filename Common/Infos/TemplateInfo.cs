using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos;

public class TemplateInfo : ReferenceDataElementInfo
{
    public List<Guid> EntriesIds { get; } = new();
}