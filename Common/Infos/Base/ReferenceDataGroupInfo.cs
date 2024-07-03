using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

public class ReferenceDataGroupInfo : ReferenceDataInfo, IReferenceDataGroupInfo
{
    public Guid? ParentId { get; set; }
    public List<Guid> ChildrenIds { get; } = new();
    public List<Guid> ElementsIds { get; } = new();
}