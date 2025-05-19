namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IGroupEntity<TGroup, TElement> : IEntity
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement : class, IElementEntity<TGroup, TElement>
{
    TGroup Parent { get; set; }
    Guid ParentId { get; set; }
    ICollection<TGroup> Children { get; }
    ICollection<TElement> Elements { get; }
}