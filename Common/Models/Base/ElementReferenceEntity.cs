using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Base;

public abstract class ElementReferenceEntity<TGroup, TElement> : ReferenceEntity, IElementReferenceEntity<TGroup, TElement>
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement : class, IElementReferenceEntity<TGroup, TElement>
{
    public Guid GroupId { get; set; }
    public TGroup Group { get; set; }
}