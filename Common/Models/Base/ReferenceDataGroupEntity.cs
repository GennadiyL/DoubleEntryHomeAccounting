using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataGroupEntity<TGroup, TElement> : ReferenceDataEntity, IReferenceDataGroupEntity<TGroup, TElement>
    where TGroup : IGroupEntity<TGroup, TElement>
{
    public TGroup Parent { get; set; }
    public IList<TGroup> Children { get; } = new List<TGroup>();
    public IList<TElement> Elements { get; } = new List<TElement>();
}