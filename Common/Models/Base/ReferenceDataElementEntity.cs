using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataElementEntity<TGroup> : ReferenceDataEntity, IReferenceDataElementEntity<TGroup>
{
    public TGroup Group { get; set; }
}