using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

public class ElementInfo : ReferenceInfo, IElementInfo
{
    public Guid GroupId { get; set; }
}