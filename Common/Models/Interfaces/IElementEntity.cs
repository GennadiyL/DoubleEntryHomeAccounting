namespace GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

public interface IElementEntity<TGroup, TElement> : IEntity
    where TGroup : class, IGroupEntity<TGroup, TElement>
    where TElement: class, IElementEntity<TGroup, TElement>
{
    TGroup Group { get; set; }
    Guid GroupId { get; set; }
}