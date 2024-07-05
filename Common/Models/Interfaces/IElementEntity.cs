namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IElementEntity<TGroup, TElement> : IEntity
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement: class, IElementEntity<TGroup, TElement>
{
    Guid GroupId { get; set; }
    TGroup Group { get; set; }
}