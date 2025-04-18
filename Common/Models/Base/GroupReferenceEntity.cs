using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class GroupReferenceEntity<TGroup, TElement> : ReferenceEntity, IGroupReferenceEntity<TGroup, TElement>
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement : class, IElementReferenceEntity<TGroup, TElement>
{
    public Guid? ParentId { get; set; }
    public TGroup Parent { get; set; }
    public ICollection<TGroup> Children { get; } = new List<TGroup>();
    public ICollection<TElement> Elements { get; } = new List<TElement>();
}