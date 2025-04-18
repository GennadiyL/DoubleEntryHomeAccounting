namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IGroupReferenceEntity<TGroup, TElement> : IEntity
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement : class, IElementReferenceEntity<TGroup, TElement>
{
    Guid? ParentId { get; set; }
    TGroup Parent { get; set; }
    ICollection<TGroup> Children { get; }
    ICollection<TElement> Elements { get; }
}