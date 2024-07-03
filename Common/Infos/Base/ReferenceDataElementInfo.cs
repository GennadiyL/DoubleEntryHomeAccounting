using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

public class ReferenceDataElementInfo : ReferenceDataInfo, IReferenceDataElementInfo
{
    public Guid GroupId { get; set; }
}