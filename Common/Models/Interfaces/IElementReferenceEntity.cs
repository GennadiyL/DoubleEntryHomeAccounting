namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IElementReferenceEntity<TGroup, TElement> : IEntity
    where TGroup : class, IGroupReferenceEntity<TGroup, TElement>
    where TElement: class, IElementReferenceEntity<TGroup, TElement>
{
    TGroup Group { get; set; }
    Guid GroupId { get; set; }
}