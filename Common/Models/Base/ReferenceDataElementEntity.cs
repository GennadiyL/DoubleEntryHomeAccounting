using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataElementEntity<TGroup, TElement> : ReferenceDataEntity, IReferenceDataElementEntity<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
    public Guid GroupId { get; set; }
    public TGroup Group { get; set; }
}