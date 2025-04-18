using GLSoft.DoubleEntryHomeAccounting.Common.Infos.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infos.Base;

public class ReferenceInfo : Info, IReferenceInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public bool IsFavorite { get; set; }
}