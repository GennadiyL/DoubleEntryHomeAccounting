using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

public class ElementReferenceInfo : ReferenceInfo, IElementReferenceInfo
{
    public Guid GroupId { get; set; }
}