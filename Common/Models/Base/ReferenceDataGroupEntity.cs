using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataGroupEntity<TGroup, TElement> : ReferenceDataEntity, IReferenceDataGroupEntity<TGroup, TElement>
    where TGroup : class, IReferenceDataGroupEntity<TGroup, TElement>
    where TElement : class, IReferenceDataElementEntity<TGroup, TElement>
{
    public TGroup Parent { get; set; }
    public ICollection<TGroup> Children { get; } = new List<TGroup>();
    public ICollection<TElement> Elements { get; } = new List<TElement>();
}