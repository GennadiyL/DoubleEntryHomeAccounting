using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ReferenceDataGroupEntity<TGroup, TElement> : ReferenceDataEntity, IReferenceDataGroupEntity<TGroup, TElement>
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    public Guid? ParentId { get; set; }
    public TGroup Parent { get; set; }
    public ICollection<TGroup> Children { get; } = new List<TGroup>();
    public ICollection<TElement> Elements { get; } = new List<TElement>();
}